using CertificatesWebApp.Certificates.Repositories;
using CertificatesWebApp.Infrastructure;
using Data.Models;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace CertificatesWebApp.Users.Services
{
    public interface ICertificateService : IService<Certificate>
    {
        void AcceptCertificate(Guid certificateRequestId);
        void DeclineCertificate(Guid certificateRequestId, string rejectionReason);
        Certificate SaveCertificate(Certificate certificate);
        void SaveCertificateToFileSystem(X509Certificate2 certificate, RSA rsa);
        Certificate GetBySerialNumber(String serialNumber);
        Boolean IsValid(String serialNumber);
        public IEnumerable<Certificate> GetAll();
    }
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICertificateRequestRepository _certificateRequestRepository;
        private readonly IUserService _userService;

        public CertificateService(ICertificateRepository certificateRepository, ICertificateRequestRepository certificateRequestRepository, IUserService userService)
        {
            _certificateRepository = certificateRepository;
            _certificateRequestRepository = certificateRequestRepository;
            _userService = userService;
        }
        public void AcceptCertificate(Guid certificateRequestId)   
        {
            Data.Models.CertificateRequest request = _certificateRequestRepository.Read(certificateRequestId);
            User user = _userService.Get(request.OwnerId);
            User issuer=null;
            if (request.ParentSerialNumber!="")
                issuer = _userService.Get(_certificateRepository.FindBySerialNumber(request.ParentSerialNumber).Result.OwnerId);


            DateTime expDate= request.EndDate;
            string algorithm = request.HashAlgorithm;
            CertificateType cType = request.Type;
            string flagsInput = request.Flags;
            string attributes = generateAttributes(user,request.SubjectText);
            X509KeyUsageFlags flags = getFlags(flagsInput);
            using (var rsa = RSA.Create(4096))
            {
                System.Security.Cryptography.X509Certificates.CertificateRequest certificateRequest = new System.Security.Cryptography.X509Certificates.CertificateRequest(attributes,rsa, new HashAlgorithmName(algorithm), RSASignaturePadding.Pkcs1);
                certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(!(cType==CertificateType.END), false, 0, true));
                certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(flags, false));

                //X509Certificate2 tempCert = certificateRequest.CreateSelfSigned(DateTimeOffset.Now.Date, expDate);
                X509Certificate2 caCertificate;

                validateCertificate(request.ParentSerialNumber, user);
                IsValid(request.ParentSerialNumber);
                if (cType == CertificateType.ROOT)
                {
                    caCertificate = certificateRequest.CreateSelfSigned(DateTimeOffset.Now.Date, expDate);
                    SaveCertificateToFileSystem(caCertificate, rsa);
                    SaveCertificate(new Certificate(caCertificate.SerialNumber, DateTime.Now.Date, expDate, cType, true, user.Id, user.Id, algorithm));
                }
                else
                {
                    X509Certificate2 issuerCertificate = new X509Certificate2($"Certs/{request.ParentSerialNumber}.crt");
                    using (RSA rsa_parent = RSA.Create())
                    {
                        rsa_parent.ImportRSAPrivateKey(File.ReadAllBytes($"Keys/{request.ParentSerialNumber}.key"), out _);
                        issuerCertificate = issuerCertificate.CopyWithPrivateKey(rsa_parent);
                    }

                    Span<byte> serialNumber = stackalloc byte[8];
                    RandomNumberGenerator.Fill(serialNumber);
                    caCertificate = certificateRequest.Create(issuerCertificate, DateTimeOffset.Now.Date, expDate, serialNumber);
                    SaveCertificateToFileSystem(caCertificate, rsa);
                    SaveCertificate(new Certificate(caCertificate.SerialNumber, DateTime.Now.Date, expDate, cType, true, issuer.Id, user.Id, algorithm));
                }
                    
               X509Certificate2UI.DisplayCertificate(caCertificate);

               

               request.State = CertificateRequestState.ACCEPTED;
                _certificateRequestRepository.Update(request);
            }

        }

        public void DeclineCertificate(Guid certificateRequestId, string rejectionReason)
        {
            Data.Models.CertificateRequest request = _certificateRequestRepository.Read(certificateRequestId);
            request.State = CertificateRequestState.REJECTED;
            request.RejectionReason = rejectionReason;
            _certificateRequestRepository.Update(request);
        }
        
        public Certificate SaveCertificate(Certificate certificate)
        {
            return _certificateRepository.Create(certificate);   
        }

        public void SaveCertificateToFileSystem(X509Certificate2 certificate,RSA rsa)
        {
            String filename = $"Certs/{certificate.SerialNumber}.crt";
            String keyFilename = $"Keys/{certificate.SerialNumber}.key";

            byte[] keyBytes = rsa.ExportRSAPrivateKey();
            byte[] certificateBytes = certificate.Export(X509ContentType.Cert);

            System.IO.File.WriteAllBytes(keyFilename, keyBytes);
            System.IO.File.WriteAllBytes(filename, certificateBytes);
        }

        public IEnumerable<Certificate> GetAll() 
        {
            return _certificateRepository.ReadAll();
        }

        private X509KeyUsageFlags getFlags(string exponents)
        {
            if (exponents.Length == 0)
                return X509KeyUsageFlags.None;
            X509KeyUsageFlags flag = X509KeyUsageFlags.None;
            List<int> numbers = new List<int>();
            foreach (string exponent in exponents.Split(','))
            {
                flag = flag | (X509KeyUsageFlags)Enum.Parse(typeof(X509KeyUsageFlags), Enum.GetName(typeof(X509KeyUsageFlags), Convert.ToInt32(Math.Pow(2, Convert.ToInt32(exponent)))));
            }
            return flag;

        }

        private String generateAttributes(User user, string attributes)
        {
            string common_name = string.Concat("CN=", user.Name, " ", user.Surname);
            string email = string.Concat("E=", user.Email);
            string telephone = string.Concat("T=", user.Telephone);
            return string.Concat(common_name, ";", email, ";", telephone, ";", attributes);

        }

        public Certificate GetBySerialNumber(String serialNumber) { 
            return _certificateRepository.FindBySerialNumber(serialNumber).Result;
        }

        public Boolean IsValid(String serialNumber) {
            if (string.IsNullOrEmpty(serialNumber))
                return false;
            Certificate certificate = _certificateRepository.FindBySerialNumber(serialNumber).Result;
            if (certificate == null || !certificate.IsValid)
                return false;
            if (DateTime.Now > certificate.EndDate)
            {
                certificate.IsValid = false;
                _certificateRepository.Update(certificate);
                return false;
            }
            return true;
        }

        private void validateCertificate(string issuerSN,User user)
        { 
            Certificate issuerCertificateInfo;
            if (!string.IsNullOrEmpty(issuerSN))
            {
                issuerCertificateInfo = _certificateRepository.FindBySerialNumber(issuerSN).Result;
                X509Certificate2 certificate = new X509Certificate2($"Certs/{issuerSN}.crt");
                using(RSA rsa = RSA.Create()) {
                    rsa.ImportRSAPrivateKey(File.ReadAllBytes($"Keys/{issuerSN}.key"),out _);
                    certificate.CopyWithPrivateKey(rsa);
                }
            }
        }
    }
}
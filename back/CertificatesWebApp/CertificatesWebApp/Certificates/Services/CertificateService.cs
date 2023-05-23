using CertificatesWebApp.Certificates.Repositories;
using CertificatesWebApp.Infrastructure;
using Data.Models;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using CertificatesWebApp.Certificates.DTOs;
using CertificateRequest = Data.Models.CertificateRequest;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using SendGrid.Helpers.Errors.Model;
using CertificatesWebApp.Exceptions;

namespace CertificatesWebApp.Users.Services
{
    public interface ICertificateService : IService<Certificate>
    {
        void AcceptCertificate(Guid certificateRequestId);
        void DeclineCertificate(Guid certificateRequestId, string rejectionReason);
        Certificate SaveCertificate(Certificate certificate);
        void SaveCertificateToFileSystem(X509Certificate2 certificate, RSA rsa);
        Certificate GetBySerialNumber(String serialNumber);
        List<Certificate> GetByParentSerialNumber(String serialNumber);
        Boolean IsValid(String serialNumber);
        public IEnumerable<Certificate> GetAllPagable(PageParametersDTO pageParameters);
        public Task<IEnumerable<Certificate>> GetAllByUserPagable(PageParametersDTO pageParameters,string userId);
        public Task<IEnumerable<Certificate>> GetAllByUser(string userId);
        public IEnumerable<Certificate> GetAll();
        GetCertificateDTO makeCertificateDTO(string serialNumber);
        void CheckOwnership(string serialNumber, string userId);
        void WithdrawCertificate(string serialNumber);
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
                    SaveCertificate(new Certificate(caCertificate.SerialNumber, DateTime.Now.Date, expDate, cType, true,Guid.Empty, user.Id, algorithm,attributes,null));
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
                    SaveCertificate(new Certificate(caCertificate.SerialNumber, DateTime.Now.Date, expDate, cType, true, issuer.Id, user.Id, algorithm, attributes,request.ParentSerialNumber));
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
        public Certificate UpdateCertificate(Certificate certificate)
        {
            return _certificateRepository.Update(certificate);
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
        public IEnumerable<Certificate> GetAllPagable(PageParametersDTO pageParameters)
        {
            return _certificateRepository.ReadAll().Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize).Take(pageParameters.PageSize);
        }
        public async Task<IEnumerable<Certificate>> GetAllByUserPagable(PageParametersDTO pageParameters, string userId)
        {
            IEnumerable<Certificate> allCertificates= await _certificateRepository.FindByOwnerId(Guid.Parse(userId));
            return allCertificates.Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize).Take(pageParameters.PageSize);
        }
        public async Task<IEnumerable<Certificate>> GetAllByUser(string userId)
        {
            IEnumerable<Certificate> allCertificates = await _certificateRepository.FindByOwnerId(Guid.Parse(userId));
            return allCertificates;
        }

        public GetCertificateDTO makeCertificateDTO(string serialNumber)
        {
            Certificate certificate = _certificateRepository.FindBySerialNumber(serialNumber).Result;
            if (certificate == null) throw new InvalidInputException("Certificate does not exist!");
            GetCertificateDTO dto = new GetCertificateDTO(certificate);
            User owner = _userService.Get(certificate.OwnerId);
            User issuer = certificate.IssuerId == Guid.Empty? null: _userService.Get(certificate.IssuerId);
            dto.Owner = owner.Name + " " + owner.Surname;
            if (issuer != null)
                dto.Issuer = issuer.Name + " " + issuer.Surname;
            return dto;
        }
        public void CheckOwnership(string serialNumber, string userId)
        {
            Certificate certificate = GetBySerialNumber(serialNumber);
            if (certificate.OwnerId.ToString()!=userId)
                throw new ResourceNotFoundException("Certificate does not exist!");
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
            string telephone = string.Concat("T=", user.Telephone).Replace("+","00");
            return string.Concat(common_name, ";", email, ";", telephone, ";", attributes);

        }

        public Certificate GetBySerialNumber(String serialNumber) { 
            return _certificateRepository.FindBySerialNumber(serialNumber).Result;
        }
        public List<Certificate> GetByParentSerialNumber(String serialNumber)
        {
            return _certificateRepository.FindByParentSerialNumber(serialNumber).Result;
        }

        public Boolean IsValid(String serialNumber) {
            if (string.IsNullOrEmpty(serialNumber))
                return false;   
            Certificate certificate = _certificateRepository.FindBySerialNumber(serialNumber).Result;
            while (certificate != null)
            {
                if (certificate == null || !certificate.IsValid)
                    return false;
                if (DateTime.Now > certificate.EndDate)
                {
                    certificate.IsValid = false;
                    _certificateRepository.Update(certificate);
                    return false;
                }
                certificate=_certificateRepository.FindBySerialNumber(certificate.ParentSerialNumber).Result;
            }
            return true;
        }

        public void WithdrawCertificate(string serialNumber)
        {
            Certificate certificate = GetBySerialNumber(serialNumber);
            List<Certificate> certificatesForProcessing = new List<Certificate>();
            certificatesForProcessing.Add(certificate);
            certificatesForProcessing.AddRange(GetByParentSerialNumber(serialNumber));
            while (certificatesForProcessing.Count > 0)
            {
                List<Certificate> newCertificatesForProcessing = new List<Certificate>();
                foreach (Certificate item in certificatesForProcessing)
                {
                    item.IsValid = false;
                    UpdateCertificate(item);
                    newCertificatesForProcessing.AddRange(GetByParentSerialNumber(item.SerialNumber));
                }
                certificatesForProcessing = newCertificatesForProcessing;
            }
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
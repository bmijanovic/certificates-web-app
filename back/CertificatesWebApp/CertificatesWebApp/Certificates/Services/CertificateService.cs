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
        void DeclineCertificate(Guid certificateRequestId);
        Certificate SaveCertificate(Certificate certificate);
        void SaveCertificateToFileSystem(X509Certificate2 certificate, RSA rsa);
    }
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;

        public CertificateService(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }
        public void AcceptCertificate(Guid certificateRequestId)   
        {
            //get user from database
            User user = new User(Guid.Parse("586c87e5-9c75-415f-9963-273bcf068f2d"), "goran", "sladic", "0653614028", "goran@uns.ac.rs", true, new List<Certificate>());
            //get issuerid
            Guid issuer_id = Guid.Parse("586c87e5-9c75-415f-9963-273bcf068f2d");
            //get request
            DateTime expDate= DateTime.Now.AddDays(20);
            string algorithm = "SHA256";
            CertificateType cType = CertificateType.INTERMEDIATE;
            string flagsInput = "1,2,4,6";
            string attributes = "O=Root";
            X509KeyUsageFlags flags = getFlags(flagsInput);
            using (var rsa = RSA.Create(4096))
            {
                System.Security.Cryptography.X509Certificates.CertificateRequest certificateRequest = new System.Security.Cryptography.X509Certificates.CertificateRequest(attributes,rsa, new HashAlgorithmName(algorithm), RSASignaturePadding.Pkcs1);
                certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(!(cType==CertificateType.END), false, 0, true));
                certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(flags, false));

                //root certificate if needed
                X509Certificate2 tempCert = certificateRequest.CreateSelfSigned(DateTimeOffset.Now.Date, expDate);
                X509Certificate2 caCertificate;


                if (cType==CertificateType.ROOT)
                    caCertificate= certificateRequest.CreateSelfSigned(DateTimeOffset.Now.Date, expDate);
                else
                {
                    Span<byte> serialNumber = stackalloc byte[8];
                    RandomNumberGenerator.Fill(serialNumber);
                    caCertificate = certificateRequest.Create(tempCert, DateTimeOffset.Now.Date, expDate, serialNumber);
                }
                    

               X509Certificate2UI.DisplayCertificate(caCertificate);

               SaveCertificateToFileSystem(caCertificate, rsa);

               //SaveCertificate(new Certificate(caCertificate.SerialNumber, DateTime.Now.Date, expDate, cType, true, issuer_id, user.Id));
            }

        }

        public void DeclineCertificate(Guid certificateRequestId)
        {
            //get certificate request

            //set certificate request to decline

            //save certificate
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
            string givenname = string.Concat("GIVENNAME=", user.Name);
            //string surname = string.Concat("SURNAME=", user.Surname);
            string email = string.Concat("E=", user.Email);
            string telephone = string.Concat("TEL=", user.Telephone);
            string uid = string.Concat("UID=", user.Id);
            return string.Concat(common_name, ";", givenname, ";"/*,surname, ";"*/, email, ";", telephone, ";", uid, ";", attributes);

        }
    }
}
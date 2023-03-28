using CertificatesWebApp.Certificates.Repositories;
using CertificatesWebApp.Infrastructure;
using Data.Models;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;

namespace CertificatesWebApp.Users.Services
{
    public interface ICertificateService : IService<Certificate>
    {
        void MakeRootCertificate();
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
        public void MakeRootCertificate()   
        {
            string user_name = "Goran";
            string user_lastname = "Sladic";
            string user_email = "sladic@uns.ac.rs";
            Guid user_id = Guid.Parse("586c87e5-9c75-415f-9963-273bcf068f2d");
            Guid issuer_id = Guid.Parse("586c87e5-9c75-415f-9963-273bcf068f2d");
            DateTime expDate= DateTime.Now.AddDays(20);
            string algorithm = "SHA256";
            CertificateType cType = CertificateType.ROOT;
            string flagsInput = "1,2";
            string attributes = "O=Root";
            X509KeyUsageFlags flags = getFlags(flagsInput);
            using (var rsa = RSA.Create(4096))
            {
                // ja generisem CN,SURNAME,GIVENNAME,E,UID dobijam O,OU,C
                System.Security.Cryptography.X509Certificates.CertificateRequest certificateRequest = new System.Security.Cryptography.X509Certificates.CertificateRequest(attributes,rsa, new HashAlgorithmName(algorithm), RSASignaturePadding.Pkcs1);
                X509Certificate2 tempCert = certificateRequest.CreateSelfSigned(DateTimeOffset.Now.Date, expDate);
                certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(!(cType==CertificateType.END), false, 0, true));
                certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(flags, false));
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

                SaveCertificate(new Certificate(caCertificate.SerialNumber, DateTime.Now.Date, expDate, cType, true, issuer_id, user_id));

                //sacuvati u bazu

                /*byte[] certData = File.ReadAllBytes(filename);
                byte[] privateKeyData = File.ReadAllBytes(keyFilename);
                Console.WriteLine(privateKeyData.Length);

                RSA newRSA = RSA.Create();
                newRSA.ImportRSAPrivateKey(privateKeyData, out _);

                X509Certificate2 cert = new X509Certificate2(certData).CopyWithPrivateKey(newRSA);
                X509Certificate2UI.DisplayCertificate(cert);*/
            }

        }
        private X509KeyUsageFlags getFlags(string exponents) {

            if (exponents.Length == 0)
                return X509KeyUsageFlags.None;
            X509KeyUsageFlags flag=X509KeyUsageFlags.None;
            List<int> numbers = new List<int>();
            foreach (string exponent in exponents.Split(','))
            {
                flag = flag|(X509KeyUsageFlags)Enum.Parse(typeof(X509KeyUsageFlags),Enum.GetName(typeof(X509KeyUsageFlags), Convert.ToInt32(Math.Pow(2,Convert.ToInt32(exponent)))));
            }
            return flag;

        }
        public Certificate SaveCertificate(Certificate certificate)
        {
            return _certificateRepository.Create(certificate);   
        }

        public void SaveCertificateToFileSystem(X509Certificate2 certificate,RSA rsa )
        {
            String filename = $"Certs/{certificate.SerialNumber}.crt";
            String keyFilename = $"Keys/{certificate.SerialNumber}.key";

            byte[] keyBytes = rsa.ExportRSAPrivateKey();
            byte[] certificateBytes = certificate.Export(X509ContentType.Cert);

            System.IO.File.WriteAllBytes(keyFilename, keyBytes);
            System.IO.File.WriteAllBytes(filename, certificateBytes);
        }
    }
}
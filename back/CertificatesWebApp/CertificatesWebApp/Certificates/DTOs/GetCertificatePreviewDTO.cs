using Data.Models;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertificatesWebApp.Certificates.DTOs
{
    public class GetCertificatePreviewDTO
    {
        public Boolean IsValid { get; set; }
        public String SerialNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String CertificateType { get; set; }
        public String IssuerAttributes { get; set; }
        public String OwnerAttributes{ get; set;}


        public GetCertificatePreviewDTO(Certificate certificate,string issuerName) {
            this.IsValid = certificate.IsValid;
            this.SerialNumber = certificate.SerialNumber;
            this.StartDate = certificate.StartDate;
            this.EndDate = certificate.EndDate;
            this.CertificateType=certificate.Type.ToString();
            this.IssuerAttributes = issuerName;
            this.OwnerAttributes = certificate.Attributes;


        }
    }
}

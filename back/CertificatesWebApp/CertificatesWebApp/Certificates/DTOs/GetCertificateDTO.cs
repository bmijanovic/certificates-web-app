using Data.Models;

namespace CertificatesWebApp.Certificates.DTOs
{
    public class GetCertificateDTO
    {
        public Boolean Valid { get; set; }
        public CertificateType Type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String SerialNumber { get; set; }
        public String Owner { get; set; }
        public String Issuer { get; set; }

        public GetCertificateDTO()
        {
            
        }

        public GetCertificateDTO(Certificate certificate)
        {
            this.Type = certificate.Type;
            this.StartTime = certificate.StartDate;
            this.EndTime = certificate.EndDate;
            this.SerialNumber = certificate.SerialNumber;
        }
    }
}

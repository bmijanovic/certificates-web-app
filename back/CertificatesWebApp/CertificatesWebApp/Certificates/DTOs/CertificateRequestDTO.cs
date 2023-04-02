using Data.Models;
using System.Text.Json.Serialization;

namespace CertificatesWebApp.Certificates.DTOs
{
    public class CertificateRequestDTO
    {
        public String ParentSerialNumber { get; set; }
        public String O { get; set; }
        public String OU { get; set; }
        public String C { get; set; }
        public DateTime EndDate { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CertificateType Type { get; set; }
        public String Flags { get; set; }
        public String HashAlgorithm { get; set; }
    }
}

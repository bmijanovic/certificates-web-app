using Data.Models;
using System.Text.Json.Serialization;


namespace CertificatesWebApp.Certificates.DTOs
{
    public class GetCertificateRequestDTO
    {
        public Guid Id { get; set; }         
        public String ParentSerialNumber { get; set; }
        public String O { get; set; }
        public String OU { get; set; }
        public String C { get; set; }
        public DateTime EndDate { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CertificateType Type { get; set; }
        public String Flags { get; set; }
        public String HashAlgorithm { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CertificateRequestState State { get; set; }

        public GetCertificateRequestDTO()
        {

        }

        public GetCertificateRequestDTO(CertificateRequest request)
        {
            this.Id = request.Id;
            this.ParentSerialNumber = request.ParentSerialNumber;
            this.EndDate = request.EndDate;
            this.Type = request.Type;
            this.Flags = request.Flags;
            this.HashAlgorithm = request.HashAlgorithm;
            if (request.SubjectText.Contains("O"))
                this.O = request.SubjectText.Split("O=")[1].Split(";")[0];
            if (request.SubjectText.Contains("OU"))
                this.OU = request.SubjectText.Split("OU=")[1].Split(";")[0];
            if (request.SubjectText.Contains("C"))
                this.C = request.SubjectText.Split("C=")[1].Split(";")[0];
            this.State = request.State;
        }
    }
}

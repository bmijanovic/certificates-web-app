namespace CertificatesWebApp.Certificates.DTOs
{
    public class AllCertificateRequestsDTO
    {
        public int TotalCount { get; set; }
        public List<GetCertificateRequestDTO> CertificatesRequest { get; set; }

        public AllCertificateRequestsDTO(int totalCount, List<GetCertificateRequestDTO> certificatesRequest)
        {
            TotalCount = totalCount;
            CertificatesRequest = certificatesRequest;
        }
    }
}

namespace CertificatesWebApp.Certificates.DTOs
{
    public class AllCertificatesDTO
    {
        public int TotalCount { get; set; }
        public List<GetCertificatePreviewDTO> Certificates { get; set; }

        public AllCertificatesDTO(int totalCount,List<GetCertificatePreviewDTO> certificates) {
            TotalCount = totalCount;
            Certificates = certificates;
        }
    }
}

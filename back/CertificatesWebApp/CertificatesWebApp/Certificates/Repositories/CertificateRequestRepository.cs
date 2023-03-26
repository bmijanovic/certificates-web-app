using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Certificates.Repositories
{
    public interface ICertificateRequestRepository : IRepository<CertificateRequest>
    {

    }
    public class CertificateRequestRepository : Repository<CertificateRequest>, ICertificateRequestRepository
    {
        public CertificateRequestRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }
    }
}

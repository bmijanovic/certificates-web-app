using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Certificates.Repositories
{
    public interface ICertificateRepository : IRepository<Certificate>
    {

    }
    public class CertificateRepository : Repository<Certificate>, ICertificateRepository
    {
        public CertificateRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }
    }
}

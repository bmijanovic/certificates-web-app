using CertificatesWebApp.Interfaces;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Certificates.Repositories
{
    public interface ICertificateRepository : IRepository<Certificate>
    {

    }
    public class CertificateRepository : ICertificateRepository
    {
        private readonly CertificatesWebAppContext _certificatesWebAppContext;

        public CertificateRepository(CertificatesWebAppContext certificatesWebAppContext)
        {
            _certificatesWebAppContext = certificatesWebAppContext;
        }
    }
}

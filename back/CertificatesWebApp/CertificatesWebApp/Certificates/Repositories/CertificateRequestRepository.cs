using CertificatesWebApp.Interfaces;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Certificates.Repositories
{
    public interface ICertificateRequestRepository : IRepository<CertificateRequest>
    {

    }
    public class CertificateRequestRepository : ICertificateRequestRepository
    {
        private readonly CertificatesWebAppContext _certificatesWebAppContext;

        public CertificateRequestRepository(CertificatesWebAppContext certificatesWebAppContext)
        {
            _certificatesWebAppContext = certificatesWebAppContext;
        }
    }
}

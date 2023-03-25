using CertificatesWebApp.Certificates.Repositories;
using CertificatesWebApp.Interfaces;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface ICertificateService : IService<Certificate>
    {

    }
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;

        public CertificateService(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }
    }
}
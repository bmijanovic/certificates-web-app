using CertificatesWebApp.Certificates.Repositories;
using Data.Models;
using CertificatesWebApp.Infrastructure;

namespace CertificatesWebApp.Users.Services
{
    public interface ICertificateRequestService : IService<CertificateRequest>
    {

    }
    public class CertificateRequestService : ICertificateRequestService
    {
        private readonly ICertificateRequestRepository _certificateRequestRepository;

        public CertificateRequestService(ICertificateRequestRepository certificateRequestRepository)
        {
            _certificateRequestRepository = certificateRequestRepository;
        }
    }
}
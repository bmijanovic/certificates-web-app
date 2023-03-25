using CertificatesWebApp.Certificates.Repositories;
using CertificatesWebApp.Interfaces;
using Data.Models;

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
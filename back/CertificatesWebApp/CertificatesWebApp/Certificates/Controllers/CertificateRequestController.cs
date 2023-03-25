using CertificatesWebApp.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace CertificatesWebApp.Certificates.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificateRequestController : ControllerBase
    {
        private readonly ICertificateRequestService _certificateRequestService;
        public CertificateRequestController(ICertificateRequestService certificateRequestService)
        {
            _certificateRequestService = certificateRequestService;
        }
    }
}


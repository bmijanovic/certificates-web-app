using CertificatesWebApp.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace CertificatesWebApp.Certificates.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        public CertificateController(ICertificateService certificateService)
        {
            _certificateService = certificateService;
        }
        [HttpPost]
        [Route("accept/{certificateRequestId}")]
        public ActionResult<Boolean> AcceptCertificate(Guid certificateRequestId)
        {
            _certificateService.AcceptCertificate(certificateRequestId);
            return Ok(true);
        }

        [HttpPost]
        [Route("decline/{certificateRequestId}")]
        public ActionResult<Boolean> DeclineCertificate(Guid certificateRequestId)
        {
            _certificateService.DeclineCertificate(certificateRequestId);
            return Ok(true);
        }

    }
}

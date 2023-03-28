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
        [Route("makeRoot/{userId}")]
        public ActionResult<Boolean> MakeRequestForRoot(Guid userId)
        {
            _certificateService.MakeRootCertificate();
            return Ok(true);
        }
    }
}

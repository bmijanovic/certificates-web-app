using CertificatesWebApp.Certificates.DTOs;
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

        [HttpPost]
        [Route("{userId}")]
        public async Task<ActionResult> MakeRequestForCertificate(Guid userId, [FromBody] CertificateRequestDTO dto)
        {
            String role = "ADMIN";
            //try
            //{
            //    await _certificateRequestService.MakeRequestForCertificate(userId, role, dto);
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.Message);
            //}
            await _certificateRequestService.MakeRequestForCertificate(userId, role, dto);

            return Ok();
        }
    }
}


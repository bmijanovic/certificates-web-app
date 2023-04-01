using CertificatesWebApp.Certificates.DTOs;
using CertificatesWebApp.Users.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize]
        [Route("{userId}")]
        public async Task<ActionResult> MakeRequestForCertificate(Guid userId, [FromBody] CertificateRequestDTO dto)
        {
            var result = await HttpContext.AuthenticateAsync();
            string claimValue = "";
            if (result.Succeeded)
            {
                var identity = result.Principal.Identity as ClaimsIdentity;
                claimValue = identity.FindFirst("Role")?.Value;
            }
            else
            {
                return BadRequest("Didn't find claim");
            }
            try
            {
                await _certificateRequestService.MakeRequestForCertificate(userId, claimValue, dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            //String role = "ADMIN"
            //await _certificateRequestService.MakeRequestForCertificate(userId, role, dto);

            return Ok();
        }
    }
}


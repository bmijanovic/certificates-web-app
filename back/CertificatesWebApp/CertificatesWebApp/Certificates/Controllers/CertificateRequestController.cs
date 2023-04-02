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
        public async Task<ActionResult> MakeRequestForCertificate([FromBody] CertificateRequestDTO dto)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                try
                {
                    ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                    String role = identity.FindFirst(ClaimTypes.Role).Value;
                    String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                    await _certificateRequestService.MakeRequestForCertificate(Guid.Parse(userId), role, dto);
                    //ako se throwuje exception sve jedno vrati OK zbog await
                    return Ok("Certificate request created successfully!");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Cookie error");
            }
        }
    }
}


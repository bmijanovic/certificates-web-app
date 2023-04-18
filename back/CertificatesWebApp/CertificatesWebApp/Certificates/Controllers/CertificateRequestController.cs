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
                ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                String role = identity.FindFirst(ClaimTypes.Role).Value;
                String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                await _certificateRequestService.MakeRequestForCertificate(Guid.Parse(userId), role, dto);
                
                return Ok("Certificate request created successfully!");
            }
            else
            {
                return BadRequest("Cookie error");
            }
        }

        [HttpGet]
        [Authorize()]
        public async Task<ActionResult<List<GetCertificateRequestDTO>>> GetRequestsForUser()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                List<GetCertificateRequestDTO> requests = await _certificateRequestService.GetAllForUser(Guid.Parse(userId));
                return Ok(requests);
            }
            else
            {
                return BadRequest("Cookie error");
            }
        }

        [HttpGet]
        [Route("getAll")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<GetCertificateRequestDTO>> GetAllRequests()
        {
            List<GetCertificateRequestDTO> requests = _certificateRequestService.GetAll();
            return Ok(requests);
        }

    }
}


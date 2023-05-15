using CertificatesWebApp.Certificates.DTOs;
using CertificatesWebApp.Users.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using CertificatesWebApp.Security;

namespace CertificatesWebApp.Certificates.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificateRequestController : ControllerBase
    {
        private readonly ICertificateRequestService _certificateRequestService;
        private readonly IGoogleCaptchaService _googleCaptchaService;
        public CertificateRequestController(ICertificateRequestService certificateRequestService, IGoogleCaptchaService googleCaptchaService)
        {
            _certificateRequestService = certificateRequestService;
            _googleCaptchaService = googleCaptchaService;
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
                bool captchaResult = await _googleCaptchaService.VerifyToken(dto.Token);
                if (!captchaResult) return BadRequest("ReCaptcha error!");
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
        public async Task<ActionResult<List<AllCertificateRequestsDTO>>> GetRequestsForUser([FromQuery] PageParametersDTO pageParameters)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                List<GetCertificateRequestDTO> allRequests = await _certificateRequestService.GetAllForUser(Guid.Parse(userId));
                List<GetCertificateRequestDTO> requests = await _certificateRequestService.GetAllForUserPagable(pageParameters, Guid.Parse(userId));
                return Ok(new AllCertificateRequestsDTO(allRequests.Count, requests));
            }
            else
            {
                return BadRequest("Cookie error");
            }
        }

        [HttpGet]
        [Route("forApproval")]
        [Authorize()]
        public async Task<ActionResult<List<AllCertificateRequestsDTO>>> GetRequestsForApproval([FromQuery] PageParametersDTO pageParameters)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                List<GetCertificateRequestDTO> allRequests = await _certificateRequestService.GetAllForApproval(Guid.Parse(userId));
                List<GetCertificateRequestDTO> requests = await _certificateRequestService.GetAllForApprovalPagable(pageParameters, Guid.Parse(userId));

                return Ok(new AllCertificateRequestsDTO(allRequests.Count, requests));
            }
            else
            {
                return BadRequest("Cookie error");
            }
        }

        [HttpGet]
        [Route("getAll")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<AllCertificateRequestsDTO>> GetAllRequests([FromQuery] PageParametersDTO pageParameters)
        {
            List<GetCertificateRequestDTO> allRequests = _certificateRequestService.GetAll();
            List<GetCertificateRequestDTO> requests = _certificateRequestService.GetAllPagable(pageParameters);

            return Ok(new AllCertificateRequestsDTO(allRequests.Count, requests));
        }

    }
}


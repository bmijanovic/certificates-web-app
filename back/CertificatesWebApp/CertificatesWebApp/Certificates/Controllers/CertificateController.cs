using CertificatesWebApp.Users.Services;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CertificatesWebApp.Certificates.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        private readonly ICertificateRequestService _certificateRequestService;
        private readonly IUserService _userService;
        public CertificateController(ICertificateService certificateService, ICertificateRequestService certificateRequestService, IUserService userService)
        {
            _certificateService = certificateService;
            _certificateRequestService = certificateRequestService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        [Route("accept/{certificateRequestId}")]
        public async Task<ActionResult<String>> AcceptCertificateAsync(Guid certificateRequestId)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                try
                {
                    ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                    String role = identity.FindFirst(ClaimTypes.Role).Value;
                    String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                    checkUserPermission(userId, role, certificateRequestId);
                    _certificateService.AcceptCertificate(certificateRequestId);

                    return Ok("Certificate accepted successfully!");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Authentication error!");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("decline/{certificateRequestId}")]
        public async Task<ActionResult<string>> DeclineCertificateAsync(Guid certificateRequestId)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                try
                {
                    ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                    String role = identity.FindFirst(ClaimTypes.Role).Value;
                    String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                    checkUserPermission(userId, role, certificateRequestId);
                    _certificateService.DeclineCertificate(certificateRequestId);

                    return Ok("Certificate declined successfully!");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            else
            {
                return BadRequest("Authentication error!");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("")]
        public async Task<ActionResult<List<Certificate>>> GetAll()
        {
            return _certificateService.GetAll().ToList();
        }

        private void checkUserPermission(String userId, String role, Guid certificateRequestId) 
        {
            CertificateRequest request = _certificateRequestService.GetCertificateRequest(certificateRequestId);
            User issuer = null;
            if (request.ParentSerialNumber != "")
                issuer = _userService.Get(_certificateService.GetBySerialNumber(request.ParentSerialNumber).OwnerId);

            if ((issuer == null && role != "Admin") || (issuer != null && issuer.Id != Guid.Parse(userId)))
            {
                throw new Exception("You don't have permission!");
            }
        }

    }
}

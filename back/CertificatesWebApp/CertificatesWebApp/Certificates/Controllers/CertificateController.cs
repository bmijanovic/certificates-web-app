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
        public async Task<ActionResult<bool>> AcceptCertificateAsync(Guid certificateRequestId)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                try
                {
                    ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                    String role = identity.FindFirst(ClaimTypes.Role).Value;
                    String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                    CertificateRequest request = _certificateRequestService.GetCertificateRequest(certificateRequestId);
                    User issuer = null;
                    if (request.ParentSerialNumber != "")
                        issuer = _userService.Get(_certificateService.GetBySerialNumber(request.ParentSerialNumber).OwnerId);

                    if ((issuer == null && role != "Admin") || (issuer != null && issuer.Id != Guid.Parse(userId))) {
                        throw new Exception("You don't have permission!");
                    }

                    _certificateService.AcceptCertificate(certificateRequestId);
                    return Ok("Certificate accepted!");
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

        [HttpPost]
        [Authorize]
        [Route("decline/{certificateRequestId}")]
        public ActionResult<Boolean> DeclineCertificate(Guid certificateRequestId)
        {
            _certificateService.DeclineCertificate(certificateRequestId);
            return Ok(true);
        }

    }
}

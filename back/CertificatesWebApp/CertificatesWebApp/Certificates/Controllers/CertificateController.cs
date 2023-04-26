using CertificatesWebApp.Certificates.DTOs;
using CertificatesWebApp.Users.Services;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using CertificateRequest = Data.Models.CertificateRequest;
using System.Web;

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
                ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                String role = identity.FindFirst(ClaimTypes.Role).Value;
                String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                checkUserPermission(userId, role, certificateRequestId);
                _certificateService.AcceptCertificate(certificateRequestId);

                return Ok("Certificate accepted successfully!");
            }
            else
            {
                return Forbid("Authentication error!");
            }
        }

        [HttpPost]
        [Authorize]
        [Route("decline/{certificateRequestId}")]
        public async Task<ActionResult<string>> DeclineCertificateAsync(Guid certificateRequestId, [FromBody] MessageDTO dto)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                String role = identity.FindFirst(ClaimTypes.Role).Value;
                String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                checkUserPermission(userId, role, certificateRequestId);
                _certificateService.DeclineCertificate(certificateRequestId,dto.Message);

                return Ok("Certificate declined successfully!");
               
            }
            else
            {
                return Forbid("Authentication error!");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("checkValidity/{serialNumber}")]
        public ActionResult<Boolean> CheckValidityCertificate(String serialNumber)
        {
            if (_certificateService.IsValid(serialNumber)) {

                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost]
        [Authorize]
        [Route("checkValidity")]
        public ActionResult<Boolean> CheckValidityCertificate(IFormFile certificate)
        {
            if (certificate.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Read the file data into a byte array
            byte[] data;
            using (BinaryReader reader = new BinaryReader(certificate.OpenReadStream()))
            {
                data = reader.ReadBytes((int)certificate.Length);
            }

            // Create an X509Certificate2 object from the byte array
            X509Certificate2 x509Certificate = new X509Certificate2(data);
            if (_certificateService.IsValid(x509Certificate.SerialNumber))
            {

                return Ok(true);
            }
            return Ok(false);
        }

        [HttpGet]
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

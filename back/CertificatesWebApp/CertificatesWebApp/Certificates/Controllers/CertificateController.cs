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
using CertificatesWebApp.Exceptions;

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
        [Authorize(Policy = "AuthorizationPolicy")]
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
        [HttpGet]
        [Authorize(Policy = "AuthorizationPolicy")]
        [Route("download/{serialNumber}")]
        public async Task<IActionResult> GetFileById(String serialNumber)
        {
            string path = $"Certs/{serialNumber}.crt";
            if (System.IO.File.Exists(path))
            {
                return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
            }
            throw new ResourceNotFoundException("Certificate not found!");
        }



        [HttpGet]
        [Authorize(Policy = "AuthorizationPolicy")]
        [Route("download/key/{serialNumber}")]
        public async Task<IActionResult> GetKeyById(String serialNumber)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                _certificateService.CheckOwnership(serialNumber,userId);
                string path = $"Keys/{serialNumber}.key";
                if (System.IO.File.Exists(path))
                {
                    return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
                }
                return NotFound();
            }
            else
            {
                return BadRequest("Cookie error");
            }
            
        }

        [HttpPost]
        [Authorize(Policy = "AuthorizationPolicy")]
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
        [Authorize(Policy = "AuthorizationPolicy")]
        [Route("checkValidity/{serialNumber}")]
        public ActionResult<GetCertificateDTO> CheckValidityCertificate(String serialNumber)
        {
            GetCertificateDTO certificateDTO = _certificateService.makeCertificateDTO(serialNumber);
            certificateDTO.Valid = _certificateService.IsValid(serialNumber);
            return Ok(certificateDTO);
        }

        [HttpGet]
        [Authorize(Policy = "AuthorizationPolicy")]
        [Route("ownership/{serialNumber}")]
        public async Task<IActionResult> CheckOwnership(String serialNumber)
        {
            try {
                AuthenticateResult result = await HttpContext.AuthenticateAsync();
                if (result.Succeeded) 
                {
                    ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                    String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _certificateService.CheckOwnership(serialNumber, userId);
                    return Ok(true);
                }
                else
                {
                    return Forbid("Authentication error!");
                }
            }
            catch (Exception ex)
            {
                return Ok(false);
            }
        }

        [HttpPost]
        [Authorize(Policy = "AuthorizationPolicy")]
        [Route("checkValidity")]
        public ActionResult<GetCertificateDTO> CheckValidityCertificate(IFormFile certificate)
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
            GetCertificateDTO certificateDTO = _certificateService.makeCertificateDTO(x509Certificate.SerialNumber);
            certificateDTO.Valid = _certificateService.IsValid(x509Certificate.SerialNumber);
            return Ok(certificateDTO);
        }

        [HttpGet]
        [Authorize(Policy = "AuthorizationPolicy")]
        [Route("")]
        public async Task<ActionResult<AllCertificatesDTO>> GetAll([FromQuery] PageParametersDTO pageParameters)
        {
   
            return new AllCertificatesDTO(_certificateService.GetAll().ToList().Count,_certificateService.GetAllPagable(pageParameters).Select(c=> new GetCertificatePreviewDTO(c,(c.IssuerId!=Guid.Empty)?_userService.Get(c.IssuerId).Name + _userService.Get(c.IssuerId).Surname:"")).ToList());

        }
        [HttpGet]
        [Authorize(Policy = "AuthorizationPolicy")]
        [Route("my")]
        public async Task<ActionResult<AllCertificatesDTO>> GetAllByLoggedUser([FromQuery] PageParametersDTO pageParameters)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                IEnumerable<Certificate> certificates = await _certificateService.GetAllByUser(userId);
                IEnumerable<Certificate> certificatesPageable = await _certificateService.GetAllByUserPagable(pageParameters,userId);
                return new AllCertificatesDTO(certificates.Count(), certificatesPageable.Select(c => new GetCertificatePreviewDTO(c, (c.IssuerId != Guid.Empty) ? _userService.Get(c.IssuerId).Name + _userService.Get(c.IssuerId).Surname : "")).ToList());
            }
            else
            {
                return Forbid("Authentication error!");
            }
        }

        [HttpGet]
        [Authorize(Policy = "AuthorizationPolicy")]
        [Route("withdraw/{serialNumber}")]
        public async Task<ActionResult<AllCertificatesDTO>> Withdraw(String serialNumber)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                String userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                String role = identity.FindFirst(ClaimTypes.Role).Value;
                Certificate certificate = _certificateService.GetBySerialNumber(serialNumber);
                if (certificate.OwnerId.ToString() != userId && role!="Admin")
                    return NotFound("Certificate does not exist");
                _certificateService.WithdrawCertificate(serialNumber);

                return NoContent();
                    
            }
            else
            {
                return Forbid("Authentication error!");
            }
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

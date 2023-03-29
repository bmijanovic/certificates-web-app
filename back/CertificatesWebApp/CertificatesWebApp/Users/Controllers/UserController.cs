using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Exceptions;
using CertificatesWebApp.Users.Services;
using Data.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CertificatesWebApp.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICredentialsService _credentialsService;
        private readonly IConfirmationService _confirmationService;
        public UserController(IUserService userService, ICredentialsService credentialsService, IConfirmationService confirmationService)
        {
            _credentialsService = credentialsService;
            _userService = userService;
            _confirmationService = confirmationService;
        }

        [HttpPost(Name = "Register")]
        public ActionResult<UserDTO> register(UserDTO userDTO)
        {
            try
            {
                User user = _userService.CreateUser(userDTO);
                return Ok(new UserDTO(user));
            }
            catch (EmailException e)
            {
                return BadRequest(e.Message);
            }
            catch (TelephoneException e) 
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost(Name = "Login")]
        public ActionResult<String> login(CredentialsDTO credentialsDTO)
        {
            try
            {
                User user = _credentialsService.Authenticate(credentialsDTO.Email, credentialsDTO.Password);
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                //change to role in future
                identity.AddClaim(new Claim("Role", user.Name));
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return Ok("Logged in successfully!");
            }
            catch (BadCredentialsException e) {
                return BadRequest(e.Message);
            }
            catch (UserNotActivatedException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost(Name = "Logout")]
        public ActionResult<String> logout()
        {
            try
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok("Logged out successfully!");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost(Name = "ActivateAccount")]
        public ActionResult<String> activateAccount(String code){
            try {
                _confirmationService.ActivateAccount(code);
                return Ok("Account verified successfully!");
            }
            catch (ConfirmationCodeException e) {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> privateEndpointAsync()
        {
            var result = await HttpContext.AuthenticateAsync();

            if (result.Succeeded)
            {
                var identity = result.Principal.Identity as ClaimsIdentity;
                var claimValue = identity.FindFirst("Role")?.Value;

                return Ok(claimValue);
            }
            else
            {
                return BadRequest("Didn't find claim");
            }

        }
    }
}

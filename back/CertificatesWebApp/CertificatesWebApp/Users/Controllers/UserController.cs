using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Services;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

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

        [HttpPost]
        public async Task<ActionResult<UserDTO>> register(UserDTO userDTO)
        {
            User user = await _userService.CreateUser(userDTO);
            return Ok(new UserDTO(user));
        }

        [HttpPost]
        public async Task<ActionResult<String>> login(CredentialsDTO credentialsDTO)
        {
            User user = await _credentialsService.Authenticate(credentialsDTO.Email, credentialsDTO.Password);
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Discriminator));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return Ok("Logged in successfully!");
        }

        [HttpPost]
        public async Task<ActionResult<String>> logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out successfully!");
        }

        [HttpPost]
        [Route("{code}")]
        public async Task<ActionResult<String>> activateAccount(int code){
            await _confirmationService.ActivateAccount(code);
            return Ok("Account activated successfully!");
        }

        [HttpPost]
        [Route("{email}")]
        public async Task<ActionResult<String>> sendResetPasswordMail(String email)
        {
            await _userService.SendPasswordResetMail(email);
            return Ok("Password reset mail sent successfully!");
        }

        [HttpPost]
        [Route("{code}")]
        public async Task<ActionResult<String>> resetPassword(int code, PasswordResetDTO passwordResetDTO)
        {
            await _confirmationService.ResetPassword(code, passwordResetDTO);
            return Ok("Password reset successfully!");
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<ActionResult<String>> doesPasswordResetCodeExists(int code)
        {
            bool exists = await _confirmationService.ConfirmationExists(code, ConfirmationType.RESET_PASSWORD);
            if (exists)
            {
                return Ok("Password reset code exists!");
            }
            return NotFound("Password reset code does not exist!");
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<string>> whoAmI()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result.Succeeded)
            {
                ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
                String role = identity.FindFirst(ClaimTypes.Role).Value;
                return Ok(JsonConvert.SerializeObject(new { role }, Formatting.Indented));
            }
            else
            {
                return BadRequest("Cookie error");
            }
        }
    }
}

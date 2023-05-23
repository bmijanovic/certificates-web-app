using CertificatesWebApp.Exceptions;
using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Services;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

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
            return Ok(new UserSimpleDTO(user));
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
        [Route("{telephone}")]
        public async Task<ActionResult<String>> sendResetPasswordSMS(String telephone)
        {
            await _userService.SendPasswordResetSMS(telephone);
            return Ok("Password reset sms sent successfully!");
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
        [HttpGet("/api/User/signin-google")]
        public IActionResult Login2()
        {
            var redirectUrl = Url.Action("GoogleCallback");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet]
        [Route("/api/User/auth/google/callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                throw new InvalidInputException("Authentication failed.");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            User user = await _userService.GetByEmail(email);
            if (user == null)
            {

                UserDTO userDTO = new UserDTO(name.Substring(0,name.IndexOf(' ')), name.Substring(name.IndexOf(' ')),email,GenerateRandomPassword(),null,VerificationType.EMAIL);
                User newUser = await _userService.CreateUser(userDTO);
            }
            else
            {
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Discriminator));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }

            return Redirect("http://localhost:5173/certificates");
        }

        private string GenerateRandomPassword()
        {
            PasswordOptions opts = new PasswordOptions()
            {
                RequiredLength = 12,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}

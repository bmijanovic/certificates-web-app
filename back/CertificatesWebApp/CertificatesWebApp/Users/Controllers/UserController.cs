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
using CertificatesWebApp.Security;

namespace CertificatesWebApp.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICredentialsService _credentialsService;
        private readonly IConfirmationService _confirmationService;
        private readonly IGoogleCaptchaService _googleCaptchaService;

        public UserController(IUserService userService, ICredentialsService credentialsService, IConfirmationService confirmationService, IGoogleCaptchaService googleCaptchaService)
        {
            _credentialsService = credentialsService;
            _userService = userService;
            _confirmationService = confirmationService;
            _googleCaptchaService = googleCaptchaService;
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> register(UserDTO userDTO)
        {
            bool captchaResult = await _googleCaptchaService.VerifyToken(userDTO.Token);
            if (!captchaResult) return BadRequest("ReCaptcha error!");
            User user = await _userService.CreateUser(userDTO);
            return Ok(new UserSimpleDTO(user));
        }

        [HttpPost]
        public async Task<ActionResult<String>> login(CredentialsDTO credentialsDTO)
        {
            bool captchaResult = await _googleCaptchaService.VerifyToken(credentialsDTO.Token);
            if (!captchaResult) return BadRequest("ReCaptcha error!");
            User user = await _credentialsService.Authenticate(credentialsDTO.Email, credentialsDTO.Password);
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Discriminator));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim("TwoFactor", "Unconfirmed"));
            identity.AddClaim(new Claim("PasswordExpired", (await _credentialsService.IsPasswordExpired(user.Id)).ToString()));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return Ok("Logged in successfully!");
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<String>> logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logged out successfully!");
        }

        [HttpPost]
        [Route("{code}")]
        public async Task<ActionResult<String>> activateAccount(int code)
        {
            await _confirmationService.ActivateAccount(code);
            return Ok("Account activated successfully!");
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<String>> sendTwoFactor(VerificationType verificationType)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
            {
                return BadRequest("Cookie error");
            }

            ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
            Claim userClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            User user = _userService.Get(Guid.Parse(userClaim.Value));

            if (verificationType == VerificationType.SMS)
            {
                await _credentialsService.SendTwoFactorSMS(user.Telephone);
            }
            else
            {
                await _credentialsService.SendTwoFactorMail(user.Email);
            }
            return Ok("Two-factor code sent successfully!");
        }

        [HttpPost]
        [Authorize]
        [Route("{email}")]
        public async Task<ActionResult<String>> sendTwoFactorMail(String email)
        {
            await _credentialsService.SendTwoFactorMail(email);
            return Ok("Two-factor mail sent successfully!");
        }

        [HttpPost]
        [Authorize]
        [Route("{telephone}")]
        public async Task<ActionResult<String>> sendTwoFactorSMS(String telephone)
        {
            await _credentialsService.SendTwoFactorSMS(telephone);
            return Ok("Two-factor SMS sent successfully!");
        }

        [HttpPost]
        [Authorize]
        [Route("{code}")]
        public async Task<ActionResult<String>> verifyTwoFactor(int code)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
            {
                return BadRequest("Cookie error");
            }
            ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;

            Claim userClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            await _confirmationService.VerifyTwoFactor(Guid.Parse(userClaim.Value), code);

            identity.TryRemoveClaim(identity.FindFirst("TwoFactor"));
            identity.AddClaim(new Claim("TwoFactor", "Confirmed"));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return Ok("Two-factor verified successfully!");
        }

        [HttpPost]
        [Route("{email}")]
        public async Task<ActionResult<String>> sendResetPasswordMail(String email)
        {
            await _credentialsService.SendPasswordResetMail(email);
            return Ok("Password reset mail sent successfully!");
        }

        [HttpPost]
        [Route("{telephone}")]
        public async Task<ActionResult<String>> sendResetPasswordSMS(String telephone)
        {
            await _credentialsService.SendPasswordResetSMS(telephone);
            return Ok("Password reset SMS sent successfully!");
        }

        [HttpPost]
        [Route("{code}")]
        public async Task<ActionResult<String>> resetPassword(int code, PasswordResetDTO passwordResetDTO)
        {
            await _confirmationService.ResetPassword(code, passwordResetDTO);
            return Ok("Password reset successfully!");
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<String>> resetPassword(PasswordResetDTO passwordResetDTO)
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
            {
                return BadRequest("Cookie error");
            }


            ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
            Claim twoFactorClaim = identity.FindFirst("TwoFactor");

            if (twoFactorClaim.Value == "Unconfirmed") {
                return Forbid("You are not two factor verified");
            }

            Claim userClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            await _credentialsService.ResetPassword(Guid.Parse(userClaim.Value), passwordResetDTO);
            identity.TryRemoveClaim(identity.FindFirst("PasswordExpired"));
            identity.AddClaim(new Claim("PasswordExpired", "False"));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return Ok("Password reset successfully!");
        }

        [HttpGet]
        [Route("{code}")]
        public async Task<ActionResult<String>> doesPasswordResetCodeExists(int code)
        {
            bool exists = await _confirmationService.ConfirmationExists(code, ConfirmationType.RESET_PASSWORD);
            if (!exists)
            {
                return NotFound("Password reset code does not exist!");
            }
            return Ok("Password reset code exists!");
        }

        [HttpGet]
        [Authorize(Policy = "AuthorizationPolicy")]
        public async Task<ActionResult<string>> whoAmI()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
            {
                return BadRequest("Cookie error");
            }
            ClaimsIdentity identity = result.Principal.Identity as ClaimsIdentity;
            String role = identity.FindFirst(ClaimTypes.Role).Value;
            return Ok(JsonConvert.SerializeObject(new { role }, Formatting.Indented));
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
                identity.AddClaim(new Claim("TwoFactor", "Unconfirmed"));
                identity.AddClaim(new Claim("PasswordExpired", "False"));
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }

            return Redirect("https://localhost:3000/certificates");
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

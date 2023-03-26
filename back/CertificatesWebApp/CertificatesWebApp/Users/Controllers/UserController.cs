using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Services;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CertificatesWebApp.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost(Name = "Register")]
        public IActionResult register(UserDTO userDTO)
        {
            User user = _userService.createUser(userDTO);
            return Ok(new UserDTO(user));
        }

        [HttpGet(Name = "Login")]
        public IActionResult login(UserDTO userDTO)
        {

            return Ok("Logged in");
        }
    }
}

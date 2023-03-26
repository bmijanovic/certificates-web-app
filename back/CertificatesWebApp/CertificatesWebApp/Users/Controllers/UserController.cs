﻿using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Exceptions;
using CertificatesWebApp.Users.Services;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using System.Net;

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
                _credentialsService.Authenticate(credentialsDTO.Email, credentialsDTO.Password);
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
    }
}

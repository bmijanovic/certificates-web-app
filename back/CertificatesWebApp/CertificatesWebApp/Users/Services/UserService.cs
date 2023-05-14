using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Repositories;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using System.Linq.Expressions;
using System.Security.Claims;

namespace CertificatesWebApp.Users.Services
{
    public interface IUserService : IService<User>
    {
        Task<User> CreateUser(UserDTO userDTO);
        User Get(Guid userId);
        Task SendPasswordResetMail(String userEmail);
        Task SendPasswordResetSMS(String telephone);

        void GoogleAuthentication(AuthenticateResult result);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly IConfirmationService _confirmationService;
        private readonly IMailService _mailService;
        private readonly ISMSService _smsService;

        public UserService(IUserRepository userRepository, ICredentialsRepository credentialsRepository,
            IConfirmationRepository confirmationRepository, IConfirmationService confirmationService, 
            IMailService mailService, ISMSService smsService)
        {
            _userRepository = userRepository;
            _confirmationService = confirmationService;
            _credentialsRepository = credentialsRepository;
            _confirmationRepository = confirmationRepository;
            _mailService = mailService;
            _smsService = smsService;
        }

        public async Task<User> CreateUser(UserDTO userDTO)
        {
            if (await _userRepository.FindByEmail(userDTO.Email) != null)
            {
                throw new ArgumentException("User with that email already exists!");
            }
            else if (await _userRepository.FindByTelephone(userDTO.Telephone) != null)
            {
                throw new ArgumentException("User with that telephone already exists!");
            }

            User user = new User();
            user.Name = userDTO.Name;
            user.Surname = userDTO.Surname;
            user.Telephone = userDTO.Telephone;
            user.Email = userDTO.Email;
            user.IsActivated = false;
            user = _userRepository.Create(user);

            Confirmation confirmation = await _confirmationService.CreateActivationConfirmation(user);

            Credentials credentials = new Credentials();
            credentials.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
            credentials.User = user;
            credentials.ExpiratonDate = DateTime.Now.AddDays(30);
            _credentialsRepository.Create(credentials);

            try
            {
                if (userDTO.VerificationType == VerificationType.EMAIL)
                {
                    await _mailService.SendActivationMail(user, confirmation.Code);
                }
                else {
                    await _smsService.SendActivationSMS(user, confirmation.Code);
                }
                return user;
            }
            catch (Exception ex)
            {
                _userRepository.Delete(user.Id);
                _confirmationRepository.Delete(confirmation.Id);
                _credentialsRepository.Delete(credentials.Id);
                throw new ArgumentException("An error with SMS or Mail service has occured!");
            }
        }

        public async Task SendPasswordResetMail(String userEmail) { 
            User user = await _userRepository.FindByEmail(userEmail);
            if (user == null)
            {
                throw new ArgumentException("User with that email does not exist!");
            }
            else
            {
                Confirmation confirmation = await _confirmationService.CreateResetPasswordConfirmation(user);
                try
                {
                    await _mailService.SendPasswordResetMail(user, confirmation.Code);
                }
                catch(Exception ex)
                {
                    _confirmationRepository.Delete(confirmation.Id);
                    throw new ArgumentException("An error with Mail service has occured!");
                }
            }

        }

        public async Task SendPasswordResetSMS(String telephone) {
            User user = await _userRepository.FindByTelephone(telephone);
            if (user == null)
            {
                throw new ArgumentException("User with that telephone does not exist!");
            }
            else
            {
                Confirmation confirmation = await _confirmationService.CreateResetPasswordConfirmation(user);
                try
                {
                    await _smsService.SendPasswordResetSMS(user, confirmation.Code);
                }
                catch (Exception ex)
                {
                    _confirmationRepository.Delete(confirmation.Id);
                    throw new ArgumentException("An error with SMS service has occured!");
                }
            }
        }

        public User Get(Guid userId)
        {
            return _userRepository.Read(userId);
        }

        public void GoogleAuthentication(AuthenticateResult result)
        {
            if (!result.Succeeded)
            {
                throw new InvalidInputException("Authentication failed.");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var phone = result.Principal.FindFirst(ClaimTypes.OtherPhone)?.Value;
        }
    }
}

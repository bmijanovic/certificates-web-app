using CertificatesWebApp.Exceptions;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Repositories;
using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace CertificatesWebApp.Users.Services
{
    public interface IUserService : IService<User>
    {
        Task<User> CreateUser(UserDTO userDTO);
        User Get(Guid userId);
        Task SendPasswordResetMail(String userEmail);
        Task SendPasswordResetSMS(String telephone);
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
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(userDTO, new ValidationContext(userDTO, null, null), validationResults, true);
            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                {
                    throw new InvalidInputException(validationResult.ErrorMessage);
                }
            }

            User potentialUser = await _userRepository.FindByEmail(userDTO.Email);
            if (potentialUser != null)
            {
                Confirmation potentialUserConfirmation = await _confirmationRepository.FindConfirmationByUserIdAndType(potentialUser.Id, ConfirmationType.ACTIVATION);
                if (potentialUserConfirmation != null && potentialUserConfirmation.ExpirationDate.CompareTo(DateTime.UtcNow) > 0)
                {
                    _confirmationRepository.Delete(potentialUserConfirmation.Id);
                    _userRepository.Delete(potentialUserConfirmation.User.Id);
                }
                else
                {
                    throw new InvalidInputException("User with that email already exists!");
                }
            }

            potentialUser = await _userRepository.FindByTelephone(userDTO.Telephone);
            if (potentialUser != null)
            {
                Confirmation potentialUserConfirmation = await _confirmationRepository.FindConfirmationByUserIdAndType(potentialUser.Id, ConfirmationType.ACTIVATION);
                if (potentialUserConfirmation != null && potentialUserConfirmation.ExpirationDate.CompareTo(DateTime.UtcNow) > 0)
                {
                    _confirmationRepository.Delete(potentialUserConfirmation.Id);
                    _userRepository.Delete(potentialUserConfirmation.User.Id);
                }
                else
                {
                    throw new InvalidInputException("User with that telephone already exists!");
                }
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
            catch (Exception)
            {
                _userRepository.Delete(user.Id);
                _confirmationRepository.Delete(confirmation.Id);
                _credentialsRepository.Delete(credentials.Id);
                throw new InvalidInputException("An error with SMS or Mail service has occured!");
            }
        }

        public async Task SendPasswordResetMail(String userEmail) { 
            User user = await _userRepository.FindByEmail(userEmail);
            if (user == null)
            {
                throw new InvalidInputException("User with that email does not exist!");
            }
            else
            {
                Confirmation confirmation = await _confirmationService.CreateResetPasswordConfirmation(user);
                try
                {
                    await _mailService.SendPasswordResetMail(user, confirmation.Code);
                }
                catch (Exception)
                {
                    _confirmationRepository.Delete(confirmation.Id);
                    throw new InvalidInputException("An error with Mail service has occured!");
                }
            }

        }

        public async Task SendPasswordResetSMS(String telephone) {
            User user = await _userRepository.FindByTelephone(telephone);
            if (user == null)
            {
                throw new InvalidInputException("User with that telephone number does not exist!");
            }
            else
            {
                Confirmation confirmation = await _confirmationService.CreateResetPasswordConfirmation(user);
                try
                {
                    await _smsService.SendPasswordResetSMS(user, confirmation.Code);
                }
                catch (Exception)
                {
                    _confirmationRepository.Delete(confirmation.Id);
                    throw new InvalidInputException("An error with SMS service has occured!");
                }
            }
        }

        public User Get(Guid userId)
        {
            return _userRepository.Read(userId);
        }
    }
}

using CertificatesWebApp.Users.Repositories;
using Data.Models;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Exceptions;

namespace CertificatesWebApp.Users.Services
{
    public interface ICredentialsService : IService<Credentials>
    {
        Task<User> Authenticate(String email, String password);
        Task SendPasswordResetMail(String userEmail);
        Task SendPasswordResetSMS(String telephone);
        Task SendTwoFactorMail(String userEmail);
        Task SendTwoFactorSMS(String telephone);
    }
    public class CredentialsService : ICredentialsService
    {
        private readonly IConfirmationService _confirmationService;
        private readonly IMailService _mailService;
        private readonly ISMSService _smsService;
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly IUserRepository _userRepository;

        public CredentialsService(ICredentialsRepository credentialsRepository, IUserRepository userRepository,
            IConfirmationRepository confirmationRepository, IConfirmationService confirmationService,
            IMailService mailService, ISMSService smsService)
        {
            _confirmationService = confirmationService;
            _smsService = smsService;
            _mailService = mailService;
            _credentialsRepository = credentialsRepository;
            _userRepository = userRepository;
            _confirmationRepository = confirmationRepository;
        }

        public async Task<User> Authenticate(String email, String password)
        {
            Credentials credentials = await _credentialsRepository.FindByEmail(email);
            if (credentials == null || !BCrypt.Net.BCrypt.Verify(password, credentials.Password))
            {
                throw new ResourceNotFoundException("Email or password is incorrect!");
            }
            else if (!credentials.User.IsActivated)
            {
                throw new InvalidInputException("User is not activated!");
            }

            return credentials.User;
        }

        public async Task SendPasswordResetMail(String userEmail)
        {
            User user = await _userRepository.FindByEmail(userEmail);
            if (user == null)
            {
                throw new InvalidInputException("User with that email does not exist!");
            }

            Confirmation confirmation = await _confirmationService.CreateResetPasswordConfirmation(user.Id);
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

        public async Task SendPasswordResetSMS(String telephone)
        {
            User user = await _userRepository.FindByTelephone(telephone);
            if (user == null)
            {
                throw new InvalidInputException("User with that telephone number does not exist!");
            }

            Confirmation confirmation = await _confirmationService.CreateResetPasswordConfirmation(user.Id);
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

        public async Task SendTwoFactorMail(String userEmail) {
            User user = await _userRepository.FindByEmail(userEmail);
            if (user == null)
            {
                throw new InvalidInputException("User with that email does not exist!");
            }

            Confirmation confirmation = await _confirmationService.CreateTwoFactorConfirmation(user.Id);
            try
            {
                await _mailService.SendTwoFactorMail(user, confirmation.Code);
            }
            catch (Exception)
            {
                _confirmationRepository.Delete(confirmation.Id);
                throw new InvalidInputException("An error with Mail service has occured!");
            }
        }

        public async Task SendTwoFactorSMS(String telephone) {
            User user = await _userRepository.FindByTelephone(telephone);
            if (user == null)
            {
                throw new InvalidInputException("User with that telephone number does not exist!");
            }

            Confirmation confirmation = await _confirmationService.CreateResetPasswordConfirmation(user.Id);
            try
            {
                await _smsService.SendTwoFactorSMS(user, confirmation.Code);
            }
            catch (Exception)
            {
                _confirmationRepository.Delete(confirmation.Id);
                throw new InvalidInputException("An error with SMS service has occured!");
            }
        }
    }
}

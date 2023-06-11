using CertificatesWebApp.Users.Repositories;
using Data.Models;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Exceptions;
using CertificatesWebApp.Users.Dtos;

namespace CertificatesWebApp.Users.Services
{
    public interface ICredentialsService : IService<Credentials>
    {
        Task<User> Authenticate(String email, String password);
        Task<bool> IsPasswordExpired(Guid userId);
        Task ResetPassword(Guid userId, PasswordResetDTO passwordResetDTO);
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
        private readonly IPasswordRecordRepository _passwordRecordRepository;
        private readonly IUserRepository _userRepository;

        public CredentialsService(ICredentialsRepository credentialsRepository, IUserRepository userRepository,
            IConfirmationRepository confirmationRepository, IPasswordRecordRepository passwordRecordRepository,
            IConfirmationService confirmationService, IMailService mailService, ISMSService smsService)
        {
            _confirmationService = confirmationService;
            _smsService = smsService;
            _mailService = mailService;
            _credentialsRepository = credentialsRepository;
            _userRepository = userRepository;
            _passwordRecordRepository = passwordRecordRepository;
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

        public async Task<bool> IsPasswordExpired(Guid userId) { 
            return ((await _credentialsRepository.FindByUserId(userId)).ExpiratonDate.CompareTo(DateTime.UtcNow) <= 0);
        }

        public async Task ResetPassword(Guid userId, PasswordResetDTO passwordResetDTO)
        {
            if (passwordResetDTO.Password != passwordResetDTO.PasswordConfirmation)
            {
                throw new InvalidInputException("Passwords are not same!");
            }

            Credentials credentials = await _credentialsRepository.FindByUserId(userId);

            if (BCrypt.Net.BCrypt.Verify(passwordResetDTO.Password, credentials.Password))
            {
                throw new InvalidInputException("You can't use current password!");
            }

            if (await _passwordRecordRepository.IsPasswordAlreadyUsed(credentials.User.Id, passwordResetDTO.Password))
            {
                throw new InvalidInputException("You can't use old password!");
            }
            
            await _passwordRecordRepository.DeleteOldRecordsForUser(credentials.User.Id, 2);
            PasswordRecord passwordRecord = new PasswordRecord();
            passwordRecord.User = credentials.User;
            passwordRecord.DateChanged = DateTime.UtcNow;
            passwordRecord.Password = credentials.Password;
            _passwordRecordRepository.Create(passwordRecord);

            credentials.Password = BCrypt.Net.BCrypt.HashPassword(passwordResetDTO.Password);
            credentials.ExpiratonDate = DateTime.Now.AddMonths(3);
            _credentialsRepository.Update(credentials);
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

            Confirmation confirmation = await _confirmationService.CreateTwoFactorConfirmation(user.Id);
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

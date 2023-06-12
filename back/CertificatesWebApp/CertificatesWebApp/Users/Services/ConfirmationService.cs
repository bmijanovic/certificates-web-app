using CertificatesWebApp.Exceptions;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Repositories;
using Data.Models;
using System.Security.Cryptography;

namespace CertificatesWebApp.Users.Services
{
    public interface IConfirmationService : IService<Confirmation>
    {
        Task ActivateAccount(int code);
        Task ResetPassword(int code, PasswordResetDTO passwordResetDTO);
        Task VerifyTwoFactor(Guid userId, int code);
        Task<Confirmation> CreateActivationConfirmation(Guid userId);
        Task<Confirmation> CreateResetPasswordConfirmation(Guid userId);
        Task<Confirmation> CreateTwoFactorConfirmation(Guid userId);
        Task<bool> ConfirmationExists(int code, ConfirmationType confirmationType);
    }
    public class ConfirmationService : IConfirmationService
    {
        private const int VerificationCodeLength = 8;
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly IPasswordRecordRepository _passwordRecordRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SMSService> _logger;

        public ConfirmationService(IConfirmationRepository confirmationRepository, IUserRepository userRepository,
            IPasswordRecordRepository passwordRecordRepository,  ICredentialsRepository credentialsRepository, ILogger<SMSService> logger)
        {
            _confirmationRepository = confirmationRepository;
            _credentialsRepository = credentialsRepository;
            _passwordRecordRepository = passwordRecordRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task ActivateAccount(int code)
        {
            Confirmation confirmation = await _confirmationRepository.FindConfirmationByCodeAndType(code, ConfirmationType.ACTIVATION);
            if (confirmation == null)
            {
                _logger.LogError("Account activation for code {@Code} failed because code is invalid", code);
                throw new ResourceNotFoundException("Activation code invalid!");
            }

            if (confirmation.ExpirationDate.CompareTo(DateTime.UtcNow) <= 0)
            {
                _confirmationRepository.Delete(confirmation.Id);
                _userRepository.Delete(confirmation.User.Id);
                _logger.LogError("Account activation for code {@Code} failed because code is expired", code);
                throw new ResourceNotFoundException("Activation code expired, please sign up again!");
            }

            confirmation.User.IsActivated = true;
            _userRepository.Update(confirmation.User);
            _confirmationRepository.Delete(confirmation.Id);
            _logger.LogError("Account activation for user {@Id} successful", confirmation.User.Id);
        }


        public async Task ResetPassword(int code, PasswordResetDTO passwordResetDTO)
        {
            if (passwordResetDTO.Password != passwordResetDTO.PasswordConfirmation)
            {
                _logger.LogError("Password reset for code {@Code} failed because new password and password confirmation is not the same", code);
                throw new InvalidInputException("Passwords are not same!");
            }

            Confirmation confirmation = await _confirmationRepository.FindConfirmationByCodeAndType(code, ConfirmationType.RESET_PASSWORD);
            if (confirmation == null)
            {
                _logger.LogError("Password reset for code {@Code} failed because code is invalid", code);
                throw new ResourceNotFoundException("Password reset code invalid!");
            }

            if (confirmation.ExpirationDate.CompareTo(DateTime.UtcNow) <= 0)
            {
                _confirmationRepository.Delete(confirmation.Id);
                _logger.LogError("Password reset for code {@Code} failed because code is expired", code);
                throw new ResourceNotFoundException("Password reset code expired!");
            }

            Credentials credentials = await _credentialsRepository.FindByEmail(confirmation.User.Email);

            if (BCrypt.Net.BCrypt.Verify(passwordResetDTO.Password, credentials.Password))
            {
                _logger.LogError("Password reset for user {@Id} failed because new password is same as current one", credentials.User.Id);
                throw new InvalidInputException("You can't use current password!");
            }

            if (await _passwordRecordRepository.IsPasswordAlreadyUsed(credentials.User.Id, passwordResetDTO.Password))
            {
                _logger.LogError("Password reset for user {@Id} failed because new password is same as old one", credentials.User.Id);
                throw new InvalidInputException("You can't use old password!");
            }

            await _passwordRecordRepository.DeleteOldRecordsForUser(credentials.User.Id, 2);
            PasswordRecord passwordRecord = new PasswordRecord();
            passwordRecord.User = credentials.User;
            passwordRecord.DateChanged = DateTime.UtcNow;
            passwordRecord.Password = credentials.Password;
            _passwordRecordRepository.Create(passwordRecord);
            _logger.LogError("Password record for user {@Id} created successfully", passwordRecord.User.Id);

            credentials.Password = BCrypt.Net.BCrypt.HashPassword(passwordResetDTO.Password);
            credentials.ExpiratonDate = DateTime.Now.AddMonths(3);
            _credentialsRepository.Update(credentials);
            _confirmationRepository.Delete(confirmation.Id);
            _logger.LogError("Password reseted for user {@Id} successfully", passwordRecord.User.Id);
        }

        public async Task VerifyTwoFactor(Guid userId, int code) {
            User user = _userRepository.Read(userId);
            if (user == null)
            {
                _logger.LogError("Two factor verification for code {@Code} failed because user is not found", code);
                throw new ResourceNotFoundException("User not found!");
            }

            Confirmation confirmation = await _confirmationRepository.FindConfirmationByUserIdAndCodeAndType(userId, code, ConfirmationType.TWO_FACTOR);
            if (confirmation == null)
            {
                _logger.LogError("Two factor verification for code {@Code} failed because code is invalid", code);
                throw new ResourceNotFoundException("Two factor code invalid!");
            }

            if (confirmation.ExpirationDate.CompareTo(DateTime.UtcNow) <= 0)
            {
                _logger.LogError("Two factor verification for code {@Code} failed because code is expired", code);
                _confirmationRepository.Delete(confirmation.Id);
                throw new ResourceNotFoundException("Two factor code expired!");
            }
            _confirmationRepository.Delete(confirmation.Id);
        }

        public async Task<Confirmation> CreateActivationConfirmation(Guid userId) {
            User user = _userRepository.Read(userId);
            if (user == null)
            {
                _logger.LogError("Activation code creation for user {@Id} failed because user is invalid", userId);
                throw new ResourceNotFoundException("User not found!");
            }

            Confirmation confirmation = new Confirmation();
            confirmation.ConfirmationType = ConfirmationType.ACTIVATION;
            confirmation.Code = await GenerateVerificationCode(VerificationCodeLength);
            confirmation.ExpirationDate = DateTime.Now.AddHours(24);
            confirmation.User = user;
            _logger.LogError("Activation code for user {@Id} created successfully", confirmation.User.Id);
            return _confirmationRepository.Create(confirmation);
        }

        public async Task<Confirmation> CreateResetPasswordConfirmation(Guid userId)
        {
            User user = _userRepository.Read(userId);
            if (user == null)
            {
                _logger.LogError("Password reset code creation for user {@Id} failed because user is invalid", userId);
                throw new ResourceNotFoundException("User not found!");
            }

            if (!user.IsActivated)
            {
                _logger.LogError("Password reset code creation for user {@Id} failed because user is not activated", userId);
                throw new InvalidInputException("User not activated!");
            }

            await _confirmationRepository.DeleteConfirmationByUserIdAndType(user.Id, ConfirmationType.RESET_PASSWORD);
            Confirmation confirmation = new Confirmation();
            confirmation.ConfirmationType = ConfirmationType.RESET_PASSWORD;
            confirmation.Code = await GenerateVerificationCode(VerificationCodeLength);
            confirmation.ExpirationDate = DateTime.Now.AddHours(1);
            confirmation.User = user;
            _logger.LogError("Password reset code for user {@Id} created successfully", userId);
            return _confirmationRepository.Create(confirmation);
        }

        public async Task<Confirmation> CreateTwoFactorConfirmation(Guid userId)
        {
            User user = _userRepository.Read(userId);
            if (user == null)
            {
                _logger.LogError("Two factor verification code creation for user {@Id} failed because user invalid", userId);
                throw new ResourceNotFoundException("User not found!");
            }

            if (!user.IsActivated)
            {
                _logger.LogError("Two factor verification code creation for user {@Id} failed because user is not activated", userId);
                throw new InvalidInputException("User not activated!");
            }

            await _confirmationRepository.DeleteConfirmationByUserIdAndType(user.Id, ConfirmationType.TWO_FACTOR);
            Confirmation confirmation = new Confirmation();
            confirmation.ConfirmationType = ConfirmationType.TWO_FACTOR;
            confirmation.Code = await GenerateVerificationCode(VerificationCodeLength);
            confirmation.ExpirationDate = DateTime.Now.AddMinutes(10);
            confirmation.User = user;
            _logger.LogError("Two factor verification code for user {@Id} created successfully", userId);
            return _confirmationRepository.Create(confirmation);
        }

        private async Task<int> GenerateVerificationCode(int codeLength)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(codeLength);
            int verificationCode = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % (int)Math.Pow(10, codeLength);
            if (await _confirmationRepository.FindConfirmationByCode(verificationCode) != null)
            {
                return await GenerateVerificationCode(codeLength);
            }
            return verificationCode;
        }

        public async Task<bool> ConfirmationExists(int code, ConfirmationType confirmationType) {
            Confirmation confirmation = await _confirmationRepository.FindConfirmationByCodeAndType(code, confirmationType);
            if (confirmation == null)
            {
                return false;
            }
            return true;
        }

    }
}

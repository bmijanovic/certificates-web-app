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

        public ConfirmationService(IConfirmationRepository confirmationRepository, IUserRepository userRepository,
            IPasswordRecordRepository passwordRecordRepository,  ICredentialsRepository credentialsRepository)
        {
            _confirmationRepository = confirmationRepository;
            _credentialsRepository = credentialsRepository;
            _passwordRecordRepository = passwordRecordRepository;
            _userRepository = userRepository;
        }

        public async Task ActivateAccount(int code)
        {
            Confirmation confirmation = await _confirmationRepository.FindConfirmationByCodeAndType(code, ConfirmationType.ACTIVATION);
            if (confirmation == null)
            {
                throw new ResourceNotFoundException("Activation code invalid!");
            }

            if (confirmation.ExpirationDate.CompareTo(DateTime.UtcNow) > 0)
            {
                confirmation.User.IsActivated = true;
                _userRepository.Update(confirmation.User);
                _confirmationRepository.Delete(confirmation.Id);
            }
            else
            {
                _confirmationRepository.Delete(confirmation.Id);
                _userRepository.Delete(confirmation.User.Id);
                throw new ResourceNotFoundException("Activation code expired, please sign up again!");
            }
        }


        public async Task ResetPassword(int code, PasswordResetDTO passwordResetDTO)
        {
            if (passwordResetDTO.Password != passwordResetDTO.PasswordConfirmation)
            {
                throw new InvalidInputException("Passwords are not same!");
            }

            Confirmation confirmation = await _confirmationRepository.FindConfirmationByCodeAndType(code, ConfirmationType.RESET_PASSWORD);
            if (confirmation == null)
            {
                throw new ResourceNotFoundException("Password reset code invalid!");
            }

            if (confirmation.ExpirationDate.CompareTo(DateTime.UtcNow) <= 0)
            {
                _confirmationRepository.Delete(confirmation.Id);
                throw new ResourceNotFoundException("Password reset code expired!");
            }

            Credentials credentials = await _credentialsRepository.FindByEmail(confirmation.User.Email);

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
            _confirmationRepository.Delete(confirmation.Id);
        }

        public async Task VerifyTwoFactor(Guid userId, int code) {
            User user = _userRepository.Read(userId);
            if (user == null)
            {
                throw new ResourceNotFoundException("User not found!");
            }

            Confirmation confirmation = await _confirmationRepository.FindConfirmationByUserIdAndCodeAndType(userId, code, ConfirmationType.TWO_FACTOR);
            if (confirmation == null)
            {
                throw new ResourceNotFoundException("Two factor code invalid!");
            }

            if (confirmation.ExpirationDate.CompareTo(DateTime.UtcNow) > 0)
            {
                _confirmationRepository.Delete(confirmation.Id);
            }
            else
            {
                _confirmationRepository.Delete(confirmation.Id);
                throw new ResourceNotFoundException("Two factor code expired!");
            }
        }

        public async Task<Confirmation> CreateActivationConfirmation(Guid userId) {
            User user = _userRepository.Read(userId);
            if (user == null)
            {
                throw new ResourceNotFoundException("User not found!");
            }

            Confirmation confirmation = new Confirmation();
            confirmation.ConfirmationType = ConfirmationType.ACTIVATION;
            confirmation.Code = await GenerateVerificationCode(VerificationCodeLength);
            confirmation.ExpirationDate = DateTime.Now.AddHours(24);
            confirmation.User = user;
            return _confirmationRepository.Create(confirmation);
        }

        public async Task<Confirmation> CreateResetPasswordConfirmation(Guid userId)
        {
            User user = _userRepository.Read(userId);
            if (user == null)
            {
                throw new ResourceNotFoundException("User not found!");
            }

            if (!user.IsActivated)
            {
                throw new InvalidInputException("User not activated!");
            }

            await _confirmationRepository.DeleteConfirmationByUserIdAndType(user.Id, ConfirmationType.RESET_PASSWORD);
            Confirmation confirmation = new Confirmation();
            confirmation.ConfirmationType = ConfirmationType.RESET_PASSWORD;
            confirmation.Code = await GenerateVerificationCode(VerificationCodeLength);
            confirmation.ExpirationDate = DateTime.Now.AddHours(1);
            confirmation.User = user;
            return _confirmationRepository.Create(confirmation);
        }

        public async Task<Confirmation> CreateTwoFactorConfirmation(Guid userId)
        {
            User user = _userRepository.Read(userId);
            if (user == null)
            {
                throw new ResourceNotFoundException("User not found!");
            }

            if (!user.IsActivated)
            {
                throw new InvalidInputException("User not activated!");
            }

            await _confirmationRepository.DeleteConfirmationByUserIdAndType(user.Id, ConfirmationType.TWO_FACTOR);
            Confirmation confirmation = new Confirmation();
            confirmation.ConfirmationType = ConfirmationType.TWO_FACTOR;
            confirmation.Code = await GenerateVerificationCode(VerificationCodeLength);
            confirmation.ExpirationDate = DateTime.Now.AddMinutes(10);
            confirmation.User = user;
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

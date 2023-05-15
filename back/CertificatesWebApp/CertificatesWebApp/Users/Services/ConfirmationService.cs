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
        Task<Confirmation> CreateActivationConfirmation(User user);
        Task<Confirmation> CreateResetPasswordConfirmation(User user);
        Task<bool> ConfirmationExists(int code, ConfirmationType confirmationType);
    }
    public class ConfirmationService : IConfirmationService
    {
        private const int VerificationCodeLength = 8;
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly IUserRepository _userRepository;

        public ConfirmationService(IConfirmationRepository confirmationRepository, IUserRepository userRepository, ICredentialsRepository credentialsRepository)
        {
            _confirmationRepository = confirmationRepository;
            _credentialsRepository = credentialsRepository;
            _userRepository = userRepository;
        }

        public async Task ActivateAccount(int code)
        {
            Confirmation confirmation = await _confirmationRepository.FindConfirmationByCodeAndType(code, ConfirmationType.ACTIVATION);
            if (confirmation != null)
            {
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
            else {
                throw new ResourceNotFoundException("Activation code invalid");
            }
        }


        public async Task ResetPassword(int code, PasswordResetDTO passwordResetDTO)
        {
            if (passwordResetDTO.Password != passwordResetDTO.PasswordConfirmation)
            {
                throw new InvalidInputException("Passwords are not same!");
            }

            Confirmation confirmation = await _confirmationRepository.FindConfirmationByCodeAndType(code, ConfirmationType.RESET_PASSWORD);
            if (confirmation != null)
            {
                if (confirmation.ExpirationDate.CompareTo(DateTime.UtcNow) > 0)
                {
                    Credentials credentials = await _credentialsRepository.FindByEmail(confirmation.User.Email);
                    credentials.Password = BCrypt.Net.BCrypt.HashPassword(passwordResetDTO.Password);
                    credentials.ExpiratonDate = DateTime.Now.AddDays(30);
                    _credentialsRepository.Update(credentials);
                    _confirmationRepository.Delete(confirmation.Id);
                }
                else
                {
                    _confirmationRepository.Delete(confirmation.Id);
                    throw new ResourceNotFoundException("Password reset code expired!");
                }
            }
            else
            {
                throw new ResourceNotFoundException("Password reset code invalid");
            }
        }

        public async Task<Confirmation> CreateActivationConfirmation(User user) {
            Confirmation confirmation = new Confirmation();
            confirmation.ConfirmationType = ConfirmationType.ACTIVATION;
            confirmation.Code = await GenerateVerificationCode(VerificationCodeLength);
            confirmation.ExpirationDate = DateTime.Now.AddDays(1);
            confirmation.User = user;
            return _confirmationRepository.Create(confirmation);
        }

        public async Task<Confirmation> CreateResetPasswordConfirmation(User user) {
            if (user.IsActivated)
            {
                await _confirmationRepository.DeleteConfirmationByUserId(user.Id);
                Confirmation confirmation = new Confirmation();
                confirmation.ConfirmationType = ConfirmationType.RESET_PASSWORD;
                confirmation.Code = await GenerateVerificationCode(VerificationCodeLength);
                confirmation.ExpirationDate = DateTime.Now.AddDays(1);
                confirmation.User = user;
                return _confirmationRepository.Create(confirmation);
            }
            else {
                throw new InvalidInputException("User not activated!");
            }
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

using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Repositories;
using Data.Models;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;

namespace CertificatesWebApp.Users.Services
{
    public interface IConfirmationService : IService<Confirmation>
    {
        Task ActivateAccount(int code);
        Task ResetPassword(int code, PasswordResetDTO passwordResetDTO);
        Task<Confirmation> CreateActivationConfirmation(User user);
        Task<Confirmation> CreateResetPasswordConfirmation(User user);
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
            Confirmation confirmation = await _confirmationRepository.FindUserConfirmationByCodeAndType(code, ConfirmationType.ACTIVATION);
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
                    throw new KeyNotFoundException("Activation code expired!");
                }
            }
            else {
                throw new KeyNotFoundException("Activation code invalid");
            }
        }


        public async Task ResetPassword(int code, PasswordResetDTO passwordResetDTO)
        {
            if (passwordResetDTO.Password != passwordResetDTO.PasswordConfirmation)
            {
                throw new ArgumentException("Passwords are not same!");
            }

            Confirmation confirmation = await _confirmationRepository.FindUserConfirmationByCodeAndType(code, ConfirmationType.RESET_PASSWORD);
            if (confirmation != null)
            {
                if (confirmation.ExpirationDate.CompareTo(DateTime.UtcNow) > 0)
                {
                    Credentials credentials = await _credentialsRepository.FindByEmail(confirmation.User.Email);
                    credentials.Salt = BCrypt.Net.BCrypt.GenerateSalt();
                    credentials.Password = BCrypt.Net.BCrypt.HashPassword(passwordResetDTO.Password, credentials.Salt);
                    credentials.ExpiratonDate = DateTime.Now.AddDays(30);
                    _credentialsRepository.Update(credentials);
                    _confirmationRepository.Delete(confirmation.Id);
                }
                else
                {
                    _confirmationRepository.Delete(confirmation.Id);
                    throw new KeyNotFoundException("Password reset code expired!");
                }
            }
            else
            {
                throw new KeyNotFoundException("Password reset code invalid");
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
            Confirmation confirmation = new Confirmation();
            confirmation.ConfirmationType = ConfirmationType.RESET_PASSWORD;
            confirmation.Code = await GenerateVerificationCode(VerificationCodeLength);
            confirmation.ExpirationDate = DateTime.Now.AddDays(1);
            confirmation.User = user;
            return _confirmationRepository.Create(confirmation);
        }

        private async Task<int> GenerateVerificationCode(int codeLength)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(codeLength);
            int verificationCode = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % (int)Math.Pow(10, codeLength);
            if (await _confirmationRepository.FindUserConfirmationByCode(verificationCode) != null)
            {
                return await GenerateVerificationCode(codeLength);
            }
            return verificationCode;
        }

    }
}

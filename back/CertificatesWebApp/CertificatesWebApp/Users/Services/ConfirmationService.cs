using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Repositories;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface IConfirmationService : IService<Confirmation>
    {
        Task ActivateAccount(String code);
        Task ResetPassword(String code, PasswordResetDTO passwordResetDTO);
    }
    public class ConfirmationService : IConfirmationService
    {
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly IUserRepository _userRepository;

        public ConfirmationService(IConfirmationRepository confirmationRepository, IUserRepository userRepository, ICredentialsRepository credentialsRepository)
        {
            _confirmationRepository = confirmationRepository;
            _credentialsRepository = credentialsRepository;
            _userRepository = userRepository;
        }

        public async Task ActivateAccount(String code)
        {
            Confirmation confirmation = await _confirmationRepository.FindUserConfirmationByCodeAndType(code, ConfirmationType.ACTIVATION);
            if (confirmation != null)
            {
                confirmation.User.IsActivated = true;
                _userRepository.Update(confirmation.User);
                _confirmationRepository.Delete(confirmation.Id);
            }
            else {
                throw new KeyNotFoundException("Activation code invalid");
            }
        }


        public async Task ResetPassword(String code, PasswordResetDTO passwordResetDTO)
        {
            if (passwordResetDTO.Password != passwordResetDTO.PasswordConfirmation)
            {
                throw new ArgumentException("Passwords are not same!");
            }

            Confirmation confirmation = await _confirmationRepository.FindUserConfirmationByCodeAndType(code, ConfirmationType.RESET_PASSWORD);
            if (confirmation != null)
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
                throw new KeyNotFoundException("Password reset code invalid");
            }
        }
    }
}

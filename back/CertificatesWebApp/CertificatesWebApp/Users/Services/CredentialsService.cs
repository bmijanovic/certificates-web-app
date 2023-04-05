using CertificatesWebApp.Users.Repositories;
using Data.Models;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Dtos;

namespace CertificatesWebApp.Users.Services
{
    public interface ICredentialsService : IService<Credentials>
    {
        Task<User> AuthenticateAsync(String email, String password);
    }
    public class CredentialsService : ICredentialsService
    {
        private readonly ICredentialsRepository _credentialsRepository;

        public CredentialsService(ICredentialsRepository credentialsRepository)
        {
            _credentialsRepository = credentialsRepository;
        }

        public async Task<User> AuthenticateAsync(String email, String password)
        {
            Credentials credentials = await _credentialsRepository.FindByEmail(email);
            if (credentials == null || !BCrypt.Net.BCrypt.HashPassword(password, credentials.Salt).Equals(credentials.Password))
            {
                throw new KeyNotFoundException("Email or password is incorrect!");
            }
            else if (!credentials.User.IsActivated)
            {
                throw new ArgumentException("User is not activated!");
            }
            return credentials.User;
        }
    }
}

using CertificatesWebApp.Users.Repositories;
using Data.Models;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Exceptions;
using CertificatesWebApp.Users.Dtos;

namespace CertificatesWebApp.Users.Services
{
    public interface ICredentialsService : IService<Credentials>
    {
        User Authenticate(String email, String password);
    }
    public class CredentialsService : ICredentialsService
    {
        private readonly ICredentialsRepository _credentialsRepository;

        public CredentialsService(ICredentialsRepository credentialsRepository)
        {
            _credentialsRepository = credentialsRepository;
        }

        public User Authenticate(String email, String password)
        {
            Credentials credentials = _credentialsRepository.findCredentials(email);
            if (credentials == null || !BCrypt.Net.BCrypt.HashPassword(password, credentials.Salt).Equals(credentials.Password))
            {
                throw new BadCredentialsException("Email or password is incorrect!");
            }
            else if (!credentials.User.IsActivated)
            {
                throw new UserNotActivatedException("User is not activated!");
            }
            return credentials.User;
        }
    }
}

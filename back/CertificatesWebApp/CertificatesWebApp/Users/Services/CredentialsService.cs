using CertificatesWebApp.Users.Repositories;
using Data.Models;
using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Exceptions;
using CertificatesWebApp.Users.Dtos;

namespace CertificatesWebApp.Users.Services
{
    public interface ICredentialsService : IService<Credentials>
    {
        Credentials CreateCredentials(Credentials credentials);
        User Authenticate(String email, String password);
    }
    public class CredentialsService : ICredentialsService
    {
        private readonly ICredentialsRepository _credentialsRepository;

        public CredentialsService(ICredentialsRepository credentialsRepository)
        {
            _credentialsRepository = credentialsRepository;
        }

        public Credentials CreateCredentials(Credentials credentials)
        {
            return _credentialsRepository.Create(credentials);
        }

        public User Authenticate(String email, String password)
        {
            Credentials credentials = _credentialsRepository.findCredentials(email);
            String hashedInput = BCrypt.Net.BCrypt.HashPassword(password, credentials.Salt);
            if (credentials == null || !hashedInput.Equals(credentials.Password))
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

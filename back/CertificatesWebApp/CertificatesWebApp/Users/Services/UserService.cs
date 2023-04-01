using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Exceptions;
using CertificatesWebApp.Users.Repositories;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface IUserService : IService<User>
    {
        User CreateUser(UserDTO userDTO);
        User UpdateUser(User user);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly IMailService _mailService;

        public UserService(IUserRepository userRepository, ICredentialsRepository credentialsRepository,
            IConfirmationRepository confirmationRepository, IMailService mailService)
        {
            _userRepository = userRepository;
            _confirmationRepository = confirmationRepository;
            _credentialsRepository = credentialsRepository;
            _mailService = mailService;
        }

        public User CreateUser(UserDTO userDTO)
        {
            if (_userRepository.findByEmail(userDTO.Email) != null)
            {
                throw new EmailException("User with that email already exists!");
            }
            else if (_userRepository.findByTelephone(userDTO.Telephone) != null)
            {
                throw new TelephoneException("User with that telephone already exists!");
            }

            User user = new User();
            user.Name = userDTO.Name;
            user.Surname = userDTO.Surname;
            user.Telephone = userDTO.Telephone;
            user.Email = userDTO.Email;
            user.IsActivated = false;
            user = _userRepository.Create(user);

            Confirmation confirmation = new Confirmation();
            confirmation.ConfirmationType = ConfirmationType.ACTIVATION;
            confirmation.Code = Math.Abs(user.Email.GetHashCode() + DateTime.Now.GetHashCode()).ToString();
            confirmation.ExpirationDate = DateTime.Now.AddDays(1);
            confirmation.User = user;
            _confirmationRepository.Create(confirmation);

            Credentials credentials = new Credentials();
            credentials.Salt = BCrypt.Net.BCrypt.GenerateSalt();
            credentials.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password, credentials.Salt);
            credentials.User = user;
            credentials.ExpiratonDate = DateTime.Now.AddDays(30);
            _credentialsRepository.Create(credentials);

            _mailService.SendActivationMail(user, confirmation.Code);

            return user;
        }

        public User UpdateUser(User user) { 
            return _userRepository.Update(user);
        }
    }
}

using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Repositories;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface IUserService : IService<User>
    {
        Task<User> CreateUser(UserDTO userDTO);
        User Get(Guid userId);
        Task SendPasswordResetMail(String userEmail);
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

        public async Task<User> CreateUser(UserDTO userDTO)
        {
            if (await _userRepository.FindByEmail(userDTO.Email) != null)
            {
                throw new ArgumentException("User with that email already exists!");
            }
            else if (await _userRepository.FindByTelephone(userDTO.Telephone) != null)
            {
                throw new ArgumentException("User with that telephone already exists!");
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

            await _mailService.SendActivationMail(user, confirmation.Code);

            return user;
        }

        public async Task SendPasswordResetMail(String userEmail) { 
            User user = await _userRepository.FindByEmail(userEmail);
            if (user == null)
            {
                throw new ArgumentException("User does not exist!");
            }
            else {
                Confirmation confirmation = new Confirmation();
                confirmation.ConfirmationType = ConfirmationType.RESET_PASSWORD;
                confirmation.Code = Math.Abs(user.Email.GetHashCode() + DateTime.Now.GetHashCode()).ToString();
                confirmation.ExpirationDate = DateTime.Now.AddDays(1);
                confirmation.User = user;
                _confirmationRepository.Create(confirmation);

                _mailService.SendPasswordResetMail(user, confirmation.Code);
            }

        }

        public User Get(Guid userId)
        {
            return _userRepository.Read(userId);
        }
    }
}

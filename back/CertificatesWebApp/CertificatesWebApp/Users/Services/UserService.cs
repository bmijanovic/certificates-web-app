using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Dtos;
using CertificatesWebApp.Users.Repositories;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface IUserService : IService<User>
    {
        User createUser(UserDTO userDTO);
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICredentialsService _credentialsService;
        private readonly IConfirmationService _confirmationService;
        private readonly ISendGridService _sendGridService;

        public UserService(IUserRepository userRepository, ICredentialsService credentialsService, 
            IConfirmationService confirmationService, ISendGridService sendGridService)
        {
            _userRepository = userRepository;
            _credentialsService = credentialsService;
            _confirmationService = confirmationService;
            _sendGridService = sendGridService;
        }

        public User createUser(UserDTO userDTO)
        {
            User user = new User();
            user.Name = userDTO.Name;
            user.Surname = userDTO.Surname;
            user.Telephone = userDTO.Telephone;
            user.Email = userDTO.Email;
            user.IsActivated = false;
            user = _userRepository.Create(user);

            Confirmation confirmation = new Confirmation();
            confirmation.ConfirmationType = ConfirmationType.ACTIVATION;
            confirmation.Code = new Random().Next(0, 1000000).ToString("D6");
            confirmation.ExpirationDate = DateTime.Now.AddDays(1);
            confirmation.User = user;
            _confirmationService.createConfirmation(confirmation);

            Credentials credentials = new Credentials();
            credentials.Password = userDTO.Password;
            credentials.User = user;
            credentials.ExpiratonDate = DateTime.Now.AddDays(30);
            _credentialsService.createCredentials(credentials);

            //_sendGridService.sendActivationMailAsync();

            return user;
        }
    }
}

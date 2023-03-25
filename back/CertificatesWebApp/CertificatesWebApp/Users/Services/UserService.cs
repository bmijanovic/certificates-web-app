using CertificatesWebApp.Interfaces;
using CertificatesWebApp.Users.Repositories;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface IUserService : IService<User>
    {

    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}

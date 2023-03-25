using CertificatesWebApp.Interfaces;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IUserRepository : IRepository<User>
    {

    }
    public class UserRepository : IUserRepository
    {
        private readonly CertificatesWebAppContext _certificateWebAppContext;

        public UserRepository(CertificatesWebAppContext certificatesWebAppContext)
        {
            _certificateWebAppContext = certificatesWebAppContext;
        }
    }
}

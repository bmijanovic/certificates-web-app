using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User findByEmail(String email);
        User findByTelephone(String telephone);
    }
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }
        public User findByEmail(String email)
        {
            return _entities.FirstOrDefault(e => e.Email.Equals(email));
        }

        public User findByTelephone(String telephone) {
            return _entities.FirstOrDefault(e => e.Telephone.Equals(telephone));
        }
    }
}

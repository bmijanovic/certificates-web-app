using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> FindByEmail(String email);
        Task<User> FindByTelephone(String telephone);
    }
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }
        public async Task<User> FindByEmail(String email)
        {
            return await _entities.FirstOrDefaultAsync(e => e.Email.Equals(email));
        }

        public async Task<User> FindByTelephone(String telephone) {
            return await _entities.FirstOrDefaultAsync(e => e.Telephone.Equals(telephone));
        }
    }
}

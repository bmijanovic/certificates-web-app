using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Users.Repositories
{
    public interface ICredentialsRepository : IRepository<Credentials>
    {
        Task<Credentials> FindByEmail(String email);
    }
    public class CredentialsRepository : Repository<Credentials>, ICredentialsRepository
    {
        public CredentialsRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }

        public async Task<Credentials> FindByEmail(String email) {
            return await _entities.Include(e => e.User).FirstOrDefaultAsync(e => e.User.Email == email);

        }
    }
}

using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Users.Repositories
{
    public interface ICredentialsRepository : IRepository<Credentials>
    {
        Credentials findCredentials(String email);
    }
    public class CredentialsRepository : Repository<Credentials>, ICredentialsRepository
    {
        public CredentialsRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }

        public Credentials findCredentials(String email) {
            return _entities.Include(e => e.User).FirstOrDefault(e => e.User.Email == email);

        }
    }
}

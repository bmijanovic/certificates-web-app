using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Users.Repositories
{
    public interface ICredentialsRepository : IRepository<Credentials>
    {

    }
    public class CredentialsRepository : Repository<Credentials>, ICredentialsRepository
    {
        public CredentialsRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }
    }
}

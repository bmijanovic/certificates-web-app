using CertificatesWebApp.Interfaces;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Users.Repositories
{
    public interface ICredentialsRepository : IRepository<Credentials>
    {

    }
    public class CredentialsRepository : ICredentialsRepository
    {
        private readonly CertificatesWebAppContext _certificatesWebAppContext;

        public CredentialsRepository(CertificatesWebAppContext certificatesWebAppContext)
        {
            _certificatesWebAppContext = certificatesWebAppContext;
        }
    }
}

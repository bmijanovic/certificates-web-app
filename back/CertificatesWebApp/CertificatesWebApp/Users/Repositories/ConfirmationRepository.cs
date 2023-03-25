using CertificatesWebApp.Interfaces;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IConfirmationRepository : IRepository<Confirmation>
    {

    }
    public class ConfirmationRepository : IConfirmationRepository
    {
        private readonly CertificatesWebAppContext _certificatesWebAppContext;

        public ConfirmationRepository(CertificatesWebAppContext certificatesWebAppContext)
        {
            _certificatesWebAppContext = certificatesWebAppContext;
        }
    }
}

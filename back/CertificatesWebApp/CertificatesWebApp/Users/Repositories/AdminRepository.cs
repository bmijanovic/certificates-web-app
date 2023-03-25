using CertificatesWebApp.Interfaces;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IAdminRepository : IRepository<Admin>
    {

    }
    public class AdminRepository : IAdminRepository
    {
        private readonly CertificatesWebAppContext _certificateWebAppContext;

        public AdminRepository(CertificatesWebAppContext certificatesWebAppContext)
        {
            _certificateWebAppContext = certificatesWebAppContext;
        }
    }
}

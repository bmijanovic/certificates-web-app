using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IAdminRepository : IRepository<Admin>
    {

    }
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {

        public AdminRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }
    }
}

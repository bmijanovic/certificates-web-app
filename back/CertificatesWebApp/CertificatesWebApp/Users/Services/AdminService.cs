using CertificatesWebApp.Users.Repositories;
using Data.Models;
using CertificatesWebApp.Infrastructure;

namespace CertificatesWebApp.Users.Services
{
    public interface IAdminService : IService<Admin>
    {

    }
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
    }
}

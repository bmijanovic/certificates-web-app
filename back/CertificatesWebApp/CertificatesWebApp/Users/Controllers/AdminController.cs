using CertificatesWebApp.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace CertificatesWebApp.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
    }
}
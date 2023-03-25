using CertificatesWebApp.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace CertificatesWebApp.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfirmationController : ControllerBase
    {
        private readonly IConfirmationService _confirmationService;
        public ConfirmationController(IConfirmationService confirmationService)
        {
            _confirmationService = confirmationService;
        }
    }
}

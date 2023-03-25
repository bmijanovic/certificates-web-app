using CertificatesWebApp.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace CertificatesWebApp.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CredentialsController : ControllerBase
    {
        private readonly ICredentialsService _credentialsService;
        public CredentialsController(ICredentialsService credentialsService)
        {
            _credentialsService = credentialsService;
        }
    }
}

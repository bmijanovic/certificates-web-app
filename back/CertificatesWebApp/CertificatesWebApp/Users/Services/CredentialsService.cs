using CertificatesWebApp.Interfaces;
using CertificatesWebApp.Users.Repositories;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface ICredentialsService : IService<Credentials>
    {

    }
    public class CredentialsService : ICredentialsService
    {
        private readonly ICredentialsRepository _credentialsRepository;

        public CredentialsService(ICredentialsRepository credentialsRepository)
        {
            _credentialsRepository = credentialsRepository;
        }
    }
}

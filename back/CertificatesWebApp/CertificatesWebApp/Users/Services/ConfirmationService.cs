using CertificatesWebApp.Interfaces;
using CertificatesWebApp.Users.Repositories;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface IConfirmationService : IService<Confirmation>
    {

    }
    public class ConfirmationService : IConfirmationService
    {
        private readonly IConfirmationRepository _confirmationRepository;

        public ConfirmationService(IConfirmationRepository confirmationRepository)
        {
            _confirmationRepository = confirmationRepository;
        }
    }
}

using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Repositories;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface IConfirmationService : IService<Confirmation>
    {
        Confirmation CreateConfirmation(Confirmation confirmation);
    }
    public class ConfirmationService : IConfirmationService
    {
        private readonly IConfirmationRepository _confirmationRepository;

        public ConfirmationService(IConfirmationRepository confirmationRepository)
        {
            _confirmationRepository = confirmationRepository;
        }

        public Confirmation CreateConfirmation(Confirmation confirmation) { 
            return _confirmationRepository.Create(confirmation);
        }
    }
}

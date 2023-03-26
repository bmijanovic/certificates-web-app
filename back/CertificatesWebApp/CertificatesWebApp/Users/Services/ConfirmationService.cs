using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Repositories;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface IConfirmationService : IService<Confirmation>
    {
        Confirmation createConfirmation(Confirmation confirmation);
    }
    public class ConfirmationService : IConfirmationService
    {
        private readonly IConfirmationRepository _confirmationRepository;

        public ConfirmationService(IConfirmationRepository confirmationRepository)
        {
            _confirmationRepository = confirmationRepository;
        }

        public Confirmation createConfirmation(Confirmation confirmation) { 
            return _confirmationRepository.Create(confirmation);
        }
    }
}

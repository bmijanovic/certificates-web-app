using CertificatesWebApp.Infrastructure;
using CertificatesWebApp.Users.Exceptions;
using CertificatesWebApp.Users.Repositories;
using Data.Models;

namespace CertificatesWebApp.Users.Services
{
    public interface IConfirmationService : IService<Confirmation>
    {
        void ActivateAccount(String code);
    }
    public class ConfirmationService : IConfirmationService
    {
        private readonly IConfirmationRepository _confirmationRepository;
        private readonly IUserService _userService;

        public ConfirmationService(IConfirmationRepository confirmationRepository, IUserService userService)
        {
            _confirmationRepository = confirmationRepository;
            _userService = userService;
        }

        public void ActivateAccount(String code)
        {
            Confirmation confirmation = _confirmationRepository.FindUserActivationByCode(code);
            if (confirmation != null)
            {
                confirmation.User.IsActivated = true;
                _userService.UpdateUser(confirmation.User);
                _confirmationRepository.Delete(confirmation.Id);
            }
            else {
                throw new ConfirmationCodeException("Activation Code Invalid");
            }
        }
    }
}

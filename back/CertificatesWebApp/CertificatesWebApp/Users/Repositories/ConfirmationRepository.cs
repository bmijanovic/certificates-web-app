using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IConfirmationRepository : IRepository<Confirmation>
    {
        Confirmation FindUserActivationByCode(String code);
    }
    public class ConfirmationRepository : Repository<Confirmation>, IConfirmationRepository
    {
        public ConfirmationRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }

        public Confirmation FindUserActivationByCode(String code) { 
            return _entities.Include(e => e.User).FirstOrDefault(e => e.Code == code && e.ConfirmationType == ConfirmationType.ACTIVATION);
        }
    }
}

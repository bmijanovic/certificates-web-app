using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IConfirmationRepository : IRepository<Confirmation>
    {
        Task<Confirmation> FindUserConfirmationByCodeAndType(String code, ConfirmationType type);
    }
    public class ConfirmationRepository : Repository<Confirmation>, IConfirmationRepository
    {
        public ConfirmationRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }

        public async Task<Confirmation> FindUserConfirmationByCodeAndType(String code, ConfirmationType type) { 
            return await _entities.Include(e => e.User).FirstOrDefaultAsync(e => e.Code == code && e.ConfirmationType == type);
        }
    }
}

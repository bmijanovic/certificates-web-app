using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IConfirmationRepository : IRepository<Confirmation>
    {
        Task<Confirmation> FindUserConfirmationByCodeAndType(int code, ConfirmationType type);
        Task<Confirmation> FindUserConfirmationByCode(int code);
    }
    public class ConfirmationRepository : Repository<Confirmation>, IConfirmationRepository
    {
        public ConfirmationRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }

        public async Task<Confirmation> FindUserConfirmationByCodeAndType(int code, ConfirmationType type) { 
            return await _entities.Include(e => e.User).FirstOrDefaultAsync(e => e.Code == code && e.ConfirmationType == type);
        }

        public async Task<Confirmation> FindUserConfirmationByCode(int code)
        {
            return await _entities.Include(e => e.User).FirstOrDefaultAsync(e => e.Code == code);
        }
    }
}

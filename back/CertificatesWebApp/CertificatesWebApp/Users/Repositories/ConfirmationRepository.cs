using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IConfirmationRepository : IRepository<Confirmation>
    {
        Task<Confirmation> FindConfirmationByCodeAndType(int code, ConfirmationType type);
        Task<Confirmation> FindConfirmationByCode(int code);
        Task DeleteConfirmationByUserId(Guid id);
    }
    public class ConfirmationRepository : Repository<Confirmation>, IConfirmationRepository
    {
        public ConfirmationRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }

        public async Task<Confirmation> FindConfirmationByCodeAndType(int code, ConfirmationType type) { 
            return await _entities.Include(e => e.User).FirstOrDefaultAsync(e => e.Code == code && e.ConfirmationType == type);
        }

        public async Task<Confirmation> FindConfirmationByCode(int code)
        {
            return await _entities.Include(e => e.User).FirstOrDefaultAsync(e => e.Code == code);
        }

        public async Task DeleteConfirmationByUserId(Guid id) {
            _entities.RemoveRange(_entities.Where(c => c.User.Id == id));
            await _context.SaveChangesAsync();
        }
    }
}

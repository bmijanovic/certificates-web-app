using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IConfirmationRepository : IRepository<Confirmation>
    {
        Task<Confirmation> FindConfirmationByCode(int code);
        Task<Confirmation> FindConfirmationByCodeAndType(int code, ConfirmationType type);
        Task<Confirmation> FindConfirmationByUserIdAndType(Guid id, ConfirmationType type);
        Task<Confirmation> FindConfirmationByUserIdAndCodeAndType(Guid id, int code, ConfirmationType type);
        Task DeleteConfirmationByUserIdAndType(Guid id, ConfirmationType type);
    }
    public class ConfirmationRepository : Repository<Confirmation>, IConfirmationRepository
    {
        public ConfirmationRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }

        public async Task<Confirmation> FindConfirmationByCode(int code)
        {
            return await _entities.Include(e => e.User).FirstOrDefaultAsync(e => e.Code == code);
        }

        public async Task<Confirmation> FindConfirmationByCodeAndType(int code, ConfirmationType type) { 
            return await _entities.Include(e => e.User).FirstOrDefaultAsync(e => e.Code == code && e.ConfirmationType == type);
        }

        public async Task<Confirmation> FindConfirmationByUserIdAndType(Guid id, ConfirmationType type) 
        {
            return await _entities.Include(e => e.User).FirstOrDefaultAsync(e => e.User.Id == id && e.ConfirmationType == type);
        }

        public async Task<Confirmation> FindConfirmationByUserIdAndCodeAndType(Guid id, int code, ConfirmationType type) {
            return await _entities.Include(e => e.User).FirstOrDefaultAsync(e => e.User.Id == id && e.Code == code && e.ConfirmationType == type);
        }

        public async Task DeleteConfirmationByUserIdAndType(Guid id, ConfirmationType type) {
            _entities.RemoveRange(_entities.Where(c => c.User.Id == id && c.ConfirmationType == type));
            await _context.SaveChangesAsync();
        }
    }
}

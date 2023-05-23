using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Users.Repositories
{
    public interface IPasswordRecordRepository : IRepository<PasswordRecord>
    {
        Task DeleteOldRecordsForUser(Guid userId, int numberOfRecordsToRemain);
        Task<bool> IsPasswordAlreadyUsed(Guid userId, string newPassword);
    }

    public class PasswordRecordRepository : Repository<PasswordRecord>, IPasswordRecordRepository
    {
        public PasswordRecordRepository(CertificatesWebAppContext context) : base(context)
        {

        }

        public async Task DeleteOldRecordsForUser(Guid userId, int numberOfRecordsToRemain) {
            _entities.RemoveRange(_entities.Where(r => r.User.Id == userId).OrderByDescending(r => r.DateChanged).Skip(numberOfRecordsToRemain)); 
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsPasswordAlreadyUsed(Guid userId, string newPassword) {
            List<PasswordRecord> oldPasswordRecords = _entities.Where(r => r.User.Id == userId).ToList<PasswordRecord>();
            foreach (PasswordRecord oldPasswordRecord in oldPasswordRecords)
            {
                if (BCrypt.Net.BCrypt.Verify(newPassword, oldPasswordRecord.Password)) {
                    return true;
                }
            }
            return false;
        }
    }
}

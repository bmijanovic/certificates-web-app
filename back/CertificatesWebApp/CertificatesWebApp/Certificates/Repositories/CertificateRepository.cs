using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Certificates.Repositories
{
    public interface ICertificateRepository : IRepository<Certificate>
    {
        public Task<Certificate> FindBySerialNumber(string serialNumber);
        public Task<IEnumerable<Certificate>> FindByOwnerId(Guid ownerId);
        public Task<List<Certificate>> FindByParentSerialNumber(string serialNumber);
    }
    public class CertificateRepository : Repository<Certificate>, ICertificateRepository
    {
        public CertificateRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {

        }

        public async Task<Certificate> FindBySerialNumber(string serialNumber)
        {
            return await _entities.Where(e => e.SerialNumber == serialNumber).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Certificate>> FindByOwnerId(Guid ownerId)
        {
            return await _entities.Where(e => e.OwnerId == ownerId).ToListAsync();
        }
        public async Task<List<Certificate>> FindByParentSerialNumber(string serialNumber)
        {
            return await _entities.Where(e => e.ParentSerialNumber == serialNumber).ToListAsync();
        }
    }
}

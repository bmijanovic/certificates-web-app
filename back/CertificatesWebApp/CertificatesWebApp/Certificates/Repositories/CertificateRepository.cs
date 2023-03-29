using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Certificates.Repositories
{
    public interface ICertificateRepository : IRepository<Certificate>
    {
        public Task<Certificate> FindBySerialNumber(string serialNumber);
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
    }
}

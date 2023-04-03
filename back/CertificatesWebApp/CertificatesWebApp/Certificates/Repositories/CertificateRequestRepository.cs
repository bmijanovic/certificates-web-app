﻿using CertificatesWebApp.Infrastructure;
using Data.Context;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificatesWebApp.Certificates.Repositories
{
    public interface ICertificateRequestRepository : IRepository<CertificateRequest>
    {
        public Task<List<CertificateRequest>> FindByUserId(Guid userId);

    }
    public class CertificateRequestRepository : Repository<CertificateRequest>, ICertificateRequestRepository
    {
        public CertificateRequestRepository(CertificatesWebAppContext certificatesWebAppContext) : base(certificatesWebAppContext)
        {
        }

        public async Task<List<CertificateRequest>> FindByUserId(Guid userId)
        {
            return await _entities.Where(e => e.OwnerId == userId).ToListAsync();
        }
    }
}


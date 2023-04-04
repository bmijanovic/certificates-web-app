using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

namespace Data.Context
{
    public class CertificatesWebAppContext: DbContext
    {

        protected readonly IConfiguration Configuration;

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CertificateRequest> CertificatesRequests { get; set; }
        public DbSet<Confirmation> Confirmations { get; set; }
        public DbSet<Credentials> Credentials { get; set; }
        public DbSet<User> Users { get; set; }


        public CertificatesWebAppContext(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            //options.(Configuration.GetConnectionString("WebApiDatabase"));
            options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
            .Entity<CertificateRequest>()
            .Property(d => d.Type)
            .HasConversion(new EnumToStringConverter<CertificateType>());

            modelBuilder
            .Entity<CertificateRequest>()
            .Property(d => d.State)
            .HasConversion(new EnumToStringConverter<CertificateRequestState>());

            modelBuilder
            .Entity<Certificate>()
            .Property(d => d.Type)
            .HasConversion(new EnumToStringConverter<CertificateType>());
        }
    }
}

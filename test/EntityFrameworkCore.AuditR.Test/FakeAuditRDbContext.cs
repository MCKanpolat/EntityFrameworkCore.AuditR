using System;
using EntityFrameworkCore.AuditR.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.AuditR.Test
{
    public class FakeAuditRDbContext : AuditRDbContext
    {
        public FakeAuditRDbContext(Func<AuditUser> auditUserFunc, AuditRConfiguration auditRConfiguration, DbContextOptions options)
            : base(auditUserFunc, auditRConfiguration, options)
        {
        }

        public virtual DbSet<FakeDbModel> FakeDbModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
            }
        }
    }
}

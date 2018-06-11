using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.AuditR.Exceptions;
using EntityFrameworkCore.AuditR.Extensions;
using EntityFrameworkCore.AuditR.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.AuditR
{
    public class AuditRDbContext : DbContext
    {
        private readonly Func<AuditUser> _auditUserFunc;
        private readonly AuditRConfiguration _auditRConfiguration;

        public AuditRDbContext(Func<AuditUser> auditUserFunc, AuditRConfiguration auditRConfiguration, DbContextOptions options) : base(options)
        {
            _auditUserFunc = auditUserFunc ?? throw new AuditRNotRegisteredException();
            _auditRConfiguration = auditRConfiguration ?? throw new ArgumentNullException(nameof(auditRConfiguration));
        }

        public virtual DbSet<AuditEntry> AuditEntries { get; set; }
        public virtual DbSet<AuditEntryProperty> AuditEntryProperties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.MapAuditR(_auditRConfiguration);
        }

        public override int SaveChanges()
        {
            return SaveChanges(true);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess).Result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await SaveChangesAsync(true, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var currentUser = _auditUserFunc();
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser));
            }
            var correlationId = Guid.NewGuid();
            var auditEntries = new List<AuditEntry>();
            int result = 0;
            var modifiedEntries = this.GetChangeset(w => w.State == EntityState.Deleted || w.State == EntityState.Modified);
            var newEntries = this.GetChangeset(w => w.State == EntityState.Added);
            auditEntries.AddRange(this.GetAuditEntries(modifiedEntries, currentUser, correlationId));

            result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            if (modifiedEntries.Any() || newEntries.Any())
            {
                auditEntries.AddRange(this.GetAuditEntries(newEntries, currentUser, correlationId));
                AuditEntries.AddRange(auditEntries);
                await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.AuditR.Exceptions;
using EntityFrameworkCore.AuditR.Extensions;
using EntityFrameworkCore.AuditR.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.AuditR
{
    public class BeforeSavingChangesEventArgs : EventArgs
    {
        public BeforeSavingChangesEventArgs(Guid correlationId)
        {
            CorrelationId = correlationId;
            Cancel = false;
        }

        public bool Cancel { get; set; }
        public Guid CorrelationId { get; }
    }

    public class AfterSavingChangesEventArgs : EventArgs
    {
        public AfterSavingChangesEventArgs(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }
    }

    public class AuditRDbContext : DbContext
    {
        public event EventHandler<BeforeSavingChangesEventArgs> BeforeSavingChanges;
        public event EventHandler<AfterSavingChangesEventArgs> AfterSavingChanges;

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

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <exception cref="AuditUserNullException"></exception>
        /// <returns></returns>
        public override int SaveChanges()
        {
            return SaveChanges(true);
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <exception cref="AuditUserNullException"></exception>
        /// <returns></returns>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess).Result;
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <exception cref="AuditUserNullException"></exception>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveChangesAsync(true, cancellationToken);
        }

        /// <inheritdoc />
        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="AuditUserNullException"></exception>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var currentUser = _auditUserFunc();
            if (currentUser == null)
            {
                throw new AuditUserNullException();
            }
            var correlationId = Guid.NewGuid();
            if (BeforeSavingChanges != null)
            {
                var beforeSavingChangesEventArgs = new BeforeSavingChangesEventArgs(correlationId);
                BeforeSavingChanges.Invoke(this, beforeSavingChangesEventArgs);

                if (beforeSavingChangesEventArgs.Cancel)
                    return -1;
            }
            var auditEntries = new List<AuditEntry>();
            int result = 0;
            var modifiedEntries = this.GetChangeset(w => w.State == EntityState.Deleted || w.State == EntityState.Modified);
            var newEntries = this.GetChangeset(w => w.State == EntityState.Added);
            auditEntries.AddRange(modifiedEntries.GetAuditEntries(currentUser, correlationId));

            result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            if (modifiedEntries.Count > 0 || newEntries.Count > 0)
            {
                auditEntries.AddRange(newEntries.GetAuditEntries(currentUser, correlationId));
                AuditEntries.AddRange(auditEntries);
                await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }

            AfterSavingChanges?.Invoke(this, new AfterSavingChangesEventArgs(correlationId));
            return result;
        }
    }
}

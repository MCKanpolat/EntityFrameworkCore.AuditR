using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.AuditR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.AuditR.Extensions
{
    public static class DbContextExtensions
    {
        internal static List<EntityEntry> GetChangeset(this DbContext context, Func<EntityEntry, bool> predicate = null)
        {
            return context.ChangeTracker.Entries().Where(w => w.Entity is IAuditable && ((predicate == null && (w.State == EntityState.Added || w.State == EntityState.Deleted || w.State == EntityState.Modified))
                    || predicate(w)))
                .ToList();
        }

        internal static List<AuditEntry> GetAuditEntries(this IEnumerable<EntityEntry> changeset, AuditUser currentUser, Guid correlationId, bool addChangesetWhenInsert)
        {
            var dateCreated = DateTime.UtcNow;
            return changeset.Select(w => Audit(w, currentUser, correlationId, dateCreated, addChangesetWhenInsert)).ToList();
        }

        private static AuditEntry Audit(EntityEntry entry, AuditUser currentUser, Guid correlationId, DateTime dateCreated, bool addChangesetWhenInsert)
        {
            var includedProperties = new List<string>();
            var entityKey = entry.GetPrimaryKey();
            var entityType = entry.Entity.GetType();

            var auditEntry = new AuditEntry
            {
                DateCreated = dateCreated,
                EntityName = entityType.FullName,
#if NETSTANDARD2_0
                TableName = entry.Metadata.Relational().TableName,
#endif
#if NETSTANDARD2_1
                TableName = entry.Metadata.GetTableName(),
#endif
                EntityData = entry.ToJson(),
                EntityKey = entityKey,
                OperationType = entry.State.ToOperationType(),
                UserId = currentUser.Id.ToString(),
                UserName = currentUser.Name,
                IPAddress = currentUser.IpAddress,
                CorrelationId = correlationId
            };

            if (entry.State == EntityState.Modified || addChangesetWhenInsert)
            {
                var props = entityType.GetProperties().Where(prop => prop.GetCustomAttributes(typeof(AuditIgnorePropertyAttribute), false).Length == 0);
                includedProperties.AddRange(props.Select(pi => pi.Name));

                var changeset = (from prop in entry.Properties
                                 where (((addChangesetWhenInsert && prop.CurrentValue != null) || !Equals(prop.CurrentValue, prop.OriginalValue)) && includedProperties.Contains(prop.Metadata.Name))
                                 select new AuditEntryProperty
                                 {
                                     PropertyName = prop.Metadata.Name,
                                     NewValue = Convert.ToString(prop.CurrentValue),
                                     OldValue = addChangesetWhenInsert ? string.Empty : Convert.ToString(prop.OriginalValue)
                                 }).ToArray();

                auditEntry.AuditEntryProperties.AddRange(changeset);
            }
            return auditEntry;
        }
    }
}
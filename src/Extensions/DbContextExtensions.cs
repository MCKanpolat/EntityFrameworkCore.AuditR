using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.AuditR.Abstraction;
using EntityFrameworkCore.AuditR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.AuditR.Extensions;

public static class DbContextExtensions
{
    internal static EntityEntry[] GetChangeset(this DbContext context, Func<EntityEntry, bool> predicate)
    {
        return context.ChangeTracker.Entries().Where(w => w.Entity is IAuditable && predicate(w)).ToArray();
    }

    internal static async ValueTask<List<AuditEntry>> GetAuditEntriesAsync(this IEnumerable<EntityEntry> changeset,
        IServiceProvider serviceProvider, ICurrentUser currentUser,
        Guid correlationId, DateTime dateTime)
    {
        List<AuditEntry> auditEntries = [];
        foreach (var change in changeset)
        {
            var audit = await EntityAuditor.AuditAsync(serviceProvider, change, currentUser, correlationId, dateTime);
            auditEntries.Add(audit);
        }

        return auditEntries;
    }
}
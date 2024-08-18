using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.AuditR.Abstraction;
using EntityFrameworkCore.AuditR.Exceptions;
using EntityFrameworkCore.AuditR.Extensions;
using EntityFrameworkCore.AuditR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.AuditR.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;

    public AuditInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        return context == null ? result : SavingChangesAsync(eventData, result).GetAwaiter().GetResult();
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = new())
    {
        var context = eventData.Context;
        if (context is null)
        {
            return result;
        }

        var currentUser = _serviceProvider.GetService<ICurrentUser>();
        if (currentUser == null)
        {
            throw new CurrentUserNullException();
        }

        var correlationId = Guid.NewGuid();
        var dateTime = DateTime.UtcNow;

        var modifiedEntries = context.GetChangeset(w => w.State is EntityState.Deleted or EntityState.Modified);
        var newEntries = context.GetChangeset(w => w.State == EntityState.Added);

        var auditEntries =await  modifiedEntries.GetAuditEntriesAsync(_serviceProvider, currentUser, correlationId, dateTime);

        var actualResult = await base.SavingChangesAsync(eventData, result, cancellationToken);

        if (newEntries.Length > 0 || modifiedEntries.Length > 0)
        {
            if (newEntries.Length > 0)
            {
                var newAuditEntries = await newEntries.GetAuditEntriesAsync(_serviceProvider, currentUser, correlationId, dateTime);
                auditEntries.AddRange(newAuditEntries);
            }

            context.Set<AuditEntry>().AddRange(auditEntries);
            _ = await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        return actualResult;
    }
}
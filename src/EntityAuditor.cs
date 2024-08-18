using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EntityFrameworkCore.AuditR.Abstraction;
using EntityFrameworkCore.AuditR.Attributes;
using EntityFrameworkCore.AuditR.Extensions;
using EntityFrameworkCore.AuditR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.AuditR;

internal static class EntityAuditor
{
    private class EntityMetadata(string[] propertiesToIgnore, Dictionary<string, Type> customValueResolvers)
    {
        public string[] PropertiesToIgnore { get; } = propertiesToIgnore;
        public Dictionary<string, Type> CustomValueResolvers { get; } = customValueResolvers;
    }

    private static readonly ConcurrentDictionary<string, EntityMetadata> _entitiesMetadata = new();

    internal static async ValueTask<AuditEntry> AuditAsync(IServiceProvider serviceProvider, EntityEntry entry,
        ICurrentUser currentUser,
        Guid correlationId,
        DateTime dateCreated)
    {
        var auditEntry = CreateAuditEntry(entry, currentUser, correlationId, dateCreated);
        if (entry.State != EntityState.Modified)
        {
            return auditEntry;
        }

        await AuditPropertyChanges(serviceProvider, entry, auditEntry);

        return auditEntry;
    }

    private static async ValueTask AuditPropertyChanges(IServiceProvider serviceProvider, EntityEntry entry,
        AuditEntry auditEntry)
    {
        string[] propertiesToIgnore;
        Dictionary<string, Type> customValueResolvers = new();
        var entityType = entry.Entity.GetType();
        auditEntry.AuditEntryProperties = new();
        if (!_entitiesMetadata.TryGetValue(entityType.Name, out var value))
        {
            propertiesToIgnore = entityType.GetProperties().Where(prop =>
                    prop.GetCustomAttributes(typeof(AuditIgnorePropertyAttribute), false).Length == 1)
                .Select(pi => pi.Name)
                .ToArray();


            var propertiesWithResolvers = entityType.GetProperties().Select(prop =>
                new
                {
                    prop.Name,
                    ResolverAttribute =
                        prop.GetCustomAttribute(typeof(CustomValueResolverAttribute<>)) as ICustomValueResolverAttribute
                }).Where(w => w.ResolverAttribute is not null);

            foreach (var propWithResolver in propertiesWithResolvers)
            {
                if (propWithResolver.ResolverAttribute is null)
                {
                    continue;
                }

                customValueResolvers.Add(propWithResolver.Name, propWithResolver.ResolverAttribute.ResolverType);
            }

            var entityMetadata = new EntityMetadata(propertiesToIgnore, customValueResolvers);
            _entitiesMetadata.TryAdd(entityType.Name, entityMetadata);
        }
        else
        {
            propertiesToIgnore = value.PropertiesToIgnore;
            customValueResolvers = value.CustomValueResolvers;
        }

        var changeset =
            entry.Properties.Where(prop => prop.IsModified && !propertiesToIgnore.Contains(prop.Metadata.Name));

        foreach (var change in changeset)
        {
            var auditEntryProperty = new AuditEntryProperty
            {
                PropertyName = change.Metadata.Name,
            };

            if (customValueResolvers.TryGetValue(change.Metadata.Name, out var resolverType))
            {
                var valueResolver = serviceProvider.GetService(resolverType);
                if (valueResolver is null)
                {
                    throw new InvalidOperationException(
                        $"The custom value resolver {resolverType.Name} could not be resolved.");
                }

                if (valueResolver is not IAuditValueResolver resolver)
                {
                    throw new InvalidOperationException(
                        $"The custom value resolver {resolverType.Name} must implement the {nameof(IAuditValueResolver)} interface.");
                }

                auditEntryProperty.NewValue = await (resolver).Resolve(change.CurrentValue);
                auditEntryProperty.OldValue = await (resolver).Resolve(change.OriginalValue);
            }
            else
            {
                auditEntryProperty.NewValue =
                    change.CurrentValue != null ? Convert.ToString(change.CurrentValue) : null;
                auditEntryProperty.OldValue =
                    change.CurrentValue != null ? Convert.ToString(change.OriginalValue) : null;
            }

            auditEntry.AuditEntryProperties.Add(auditEntryProperty);
        }
    }

    private static AuditEntry CreateAuditEntry(EntityEntry entry, ICurrentUser currentUser, Guid correlationId,
        DateTime dateCreated)
    {
        var entityKey = entry.GetPrimaryKeyValue();
        var entityName = entry.Metadata.ShortName();

        AuditEntry auditEntry = new()
        {
            DateCreated = dateCreated,
            EntityName = entityName,
            EntityKey = entityKey,
            OperationType = entry.State.ToOperationType(),
            UserId = currentUser.Id,
            UserName = currentUser.Name,
            IPAddress = currentUser.IpAddress,
            CorrelationId = correlationId
        };
        return auditEntry;
    }
}
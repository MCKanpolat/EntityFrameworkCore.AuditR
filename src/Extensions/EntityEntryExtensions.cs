using System.Linq;
using EntityFrameworkCore.AuditR.Enums;
using EntityFrameworkCore.AuditR.Resolvers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace EntityFrameworkCore.AuditR.Extensions;

internal static class EntityEntryExtensions
{
    internal static string ToJsonString(this EntityEntry entry)
    {
        return JsonConvert.SerializeObject(entry.Entity, Formatting.Indented,
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ContractResolver = new EntityEntryContractResolver(entry)
            });
    }

    internal static string? GetPrimaryKeyValue(this EntityEntry entityEntry)
    {
        var key = entityEntry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
        return key is { CurrentValue: not null } ? key.CurrentValue.ToString() : null;
    }

    internal static OperationType ToOperationType(this EntityState entityState)
    {
        return entityState switch
        {
            EntityState.Deleted => OperationType.Delete,
            EntityState.Modified => OperationType.Update,
            EntityState.Added => OperationType.Insert,
            _ => OperationType.Insert
        };
    }
}
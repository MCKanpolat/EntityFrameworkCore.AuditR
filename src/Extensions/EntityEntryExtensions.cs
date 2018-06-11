using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace EntityFrameworkCore.AuditR.Extensions
{
    internal static class EntityEntryExtensions
    {
        internal static string ToJson(this EntityEntry entry)
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

        internal static string GetPrimaryKey(this EntityEntry entityEntry)
        {
            if (entityEntry == null)
            {
                throw new ArgumentNullException(nameof(entityEntry));
            }

            var pk = entityEntry.Metadata.FindPrimaryKey();
            var result = new List<object>();
            foreach (var prop in pk.Properties)
            {
                var value = entityEntry.Property(prop.Name).CurrentValue;
                if (value != null)
                {
                    result.Add(value);
                }
            }
            return string.Join(",", result);
        }

        internal static OperationType ToOperationType(this EntityState entityState)
        {
            switch (entityState)
            {
                case EntityState.Deleted:
                    return OperationType.Delete;
                case EntityState.Modified:
                    return OperationType.Update;
                case EntityState.Added:
                    return OperationType.Insert;
                default:
                    return OperationType.Insert;
            }
        }
    }
}

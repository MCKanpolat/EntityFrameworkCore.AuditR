using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EntityFrameworkCore.AuditR
{
    internal class EntityEntryContractResolver : DefaultContractResolver
    {
        private readonly EntityEntry _entityEntry;

        public EntityEntryContractResolver(EntityEntry entityEntry)
        {
            _entityEntry = entityEntry;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var propList = base.CreateProperties(type, memberSerialization);
            var navProps = _entityEntry.Metadata.GetNavigations().Select(n => n.Name).ToArray();
            return propList.SkipWhile(w => navProps.Any(x => x.Equals(w.PropertyName))).ToList();
        }
    }
}
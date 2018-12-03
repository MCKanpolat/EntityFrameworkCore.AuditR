using EntityFrameworkCore.AuditR.Models;
using System.Collections.Generic;

namespace EntityFrameworkCore.AuditR.Strategy
{
    public interface IAuditStrategy<T>
    {
        IEnumerable<AuditEntryProperty> Audit(T entity, AuditRDbContext auditRDbContext);
    }
}

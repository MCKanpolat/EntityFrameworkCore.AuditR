namespace EntityFrameworkCore.AuditR.Strategy
{
    public interface IAuditValueResolver
    {
        string Resolve(AuditRDbContext auditRDbContext, object value);
    }
}

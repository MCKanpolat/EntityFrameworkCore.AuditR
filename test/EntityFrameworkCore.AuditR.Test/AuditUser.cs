using EntityFrameworkCore.AuditR.Abstraction;

namespace EntityFrameworkCore.AuditR.Test;

public class AuditUser : ICurrentUser
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string IpAddress { get; set; } = null!;
}
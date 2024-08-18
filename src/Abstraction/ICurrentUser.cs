namespace EntityFrameworkCore.AuditR.Abstraction;

public interface ICurrentUser
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string IpAddress { get; set; }
}
using System.ComponentModel.DataAnnotations;
using EntityFrameworkCore.AuditR.Abstraction;
using EntityFrameworkCore.AuditR.Attributes;

namespace EntityFrameworkCore.AuditR.Test;

public class FakeDbModelWithValueResolver : IAuditable
{
    [Key] public int Id { get; set; }

    public string Name { get; set; } = null!;
    public double TestDouble { get; set; }
    public float TestFloat { get; set; }

    [CustomValueResolver<CustomAuditValueResolver>]
    public int TestInt { get; set; }
}
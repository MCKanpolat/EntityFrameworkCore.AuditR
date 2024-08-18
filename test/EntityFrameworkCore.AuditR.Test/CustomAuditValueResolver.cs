using System.Threading.Tasks;
using EntityFrameworkCore.AuditR.Abstraction;

namespace EntityFrameworkCore.AuditR.Test;

public class CustomAuditValueResolver : IAuditValueResolver
{
    private readonly string _value;

    public CustomAuditValueResolver(string value)
    {
        _value = value;
    }

    public ValueTask<string> Resolve(object? value)
    {
        return ValueTask.FromResult(_value);
    }
}
using System.Threading.Tasks;

namespace EntityFrameworkCore.AuditR.Abstraction;

public interface IAuditValueResolver
{
    ValueTask<string> Resolve(object? value);
}
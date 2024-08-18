using System;

namespace EntityFrameworkCore.AuditR.Abstraction;

public interface ICustomValueResolverAttribute
{
    Type ResolverType { get; }
}
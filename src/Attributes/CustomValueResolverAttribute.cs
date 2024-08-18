using System;
using EntityFrameworkCore.AuditR.Abstraction;

namespace EntityFrameworkCore.AuditR.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class CustomValueResolverAttribute<T> : Attribute, ICustomValueResolverAttribute
    where T : IAuditValueResolver
{
    public Type ResolverType { get; } = typeof(T);
}
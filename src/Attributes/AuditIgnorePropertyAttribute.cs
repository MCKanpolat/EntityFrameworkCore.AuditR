using System;

namespace EntityFrameworkCore.AuditR.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AuditIgnorePropertyAttribute : Attribute
{
}
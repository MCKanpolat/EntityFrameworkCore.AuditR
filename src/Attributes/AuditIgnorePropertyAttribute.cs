using System;

namespace EntityFrameworkCore.AuditR
{

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AuditIgnorePropertyAttribute : Attribute
    {
    }
}

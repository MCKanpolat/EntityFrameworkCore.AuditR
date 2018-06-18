using System;
using System.Runtime.Serialization;

namespace EntityFrameworkCore.AuditR.Exceptions
{
    [Serializable]
#pragma warning disable CA1032 // Implement standard exception constructors
    public sealed class AuditUserNullException : Exception
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        public AuditUserNullException() : base($"Current user is null.")
        {
        }

        private AuditUserNullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

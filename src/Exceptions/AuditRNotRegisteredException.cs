using System;
using System.Runtime.Serialization;

namespace EntityFrameworkCore.AuditR.Exceptions
{
    [Serializable]
#pragma warning disable CA1032 // Implement standard exception constructors
    public sealed class AuditRNotRegisteredException : Exception
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        public AuditRNotRegisteredException() : base($"Please configure AuditR in your Startup class.")
        {
        }

        private AuditRNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

using System;

namespace EntityFrameworkCore.AuditR.Exceptions;

[Serializable]
public sealed class AuditRNotConfiguredException() : Exception("Please configure AuditR in your Startup class.");
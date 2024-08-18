using System;

namespace EntityFrameworkCore.AuditR.Exceptions;

[Serializable]
public sealed class CurrentUserNullException() : Exception("Current user is null.");
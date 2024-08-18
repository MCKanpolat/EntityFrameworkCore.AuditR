using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntityFrameworkCore.AuditR.Enums;

namespace EntityFrameworkCore.AuditR.Models;

public class AuditEntry
{
    public AuditEntry()
    {
        Id = Guid.NewGuid();
    }

    [Key] public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    public DateTime DateCreated { get; set; }
    public string EntityName { get; set; } = null!;
    public string? EntityKey { get; set; }
    public string UserName { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string IPAddress { get; set; } = null!;
    public OperationType OperationType { get; set; }
    public List<AuditEntryProperty>? AuditEntryProperties { get; set; }
}
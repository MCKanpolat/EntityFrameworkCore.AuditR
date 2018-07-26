using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.AuditR.Models
{
    public class AuditEntry
    {
        public AuditEntry()
        {
            AuditEntryProperties = new List<AuditEntryProperty>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime DateCreated { get; set; }
        public string EntityName { get; set; }
        public string TableName { get; set; }
        public string EntityKey { get; set; }
        public string EntityData { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string IPAddress { get; set; }
        public OperationType OperationType { get; set; }
        public List<AuditEntryProperty> AuditEntryProperties { get; set; }
    }
}
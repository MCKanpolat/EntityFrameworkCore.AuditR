using System;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkCore.AuditR.Models
{
    public class AuditEntryProperty
    {
        public AuditEntryProperty()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string PropertyName { get; set; }
        public Guid AuditEntryId { get; set; }
        public AuditEntry AuditEntry { get; set; }
    }
}
using EntityFrameworkCore.AuditR.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EntityFrameworkCore.AuditR.Extensions
{
    internal static class ModelBuilderExtensions
    {
        internal static ModelBuilder MapAuditR(this ModelBuilder modelBuilder, AuditRConfiguration auditRConfiguration)
        {
            modelBuilder.Entity<AuditEntry>(b =>
            {
                b.Property(c => c.CorrelationId).IsRequired();
                b.Property(c => c.DateCreated).IsRequired();
                b.Property(c => c.EntityData).IsRequired();
                b.Property(c => c.EntityKey).HasMaxLength(128).IsRequired();
                b.Property(c => c.Id).IsRequired();
                b.Property(c => c.EntityName).HasMaxLength(128).IsRequired();
                b.Property(c => c.TableName).HasMaxLength(128).IsRequired();
                b.Property(c => c.UserId).HasMaxLength(38).IsRequired();
                b.Property(c => c.UserName).HasMaxLength(128).IsRequired();
                b.Property(c => c.IPAddress).HasMaxLength(45).IsRequired();
                b.Property(c => c.OperationType).HasColumnType("int").IsRequired()
                .HasConversion(v => (int)v, v => (OperationType)Enum.Parse(typeof(OperationType), v.ToString()));
            });

            modelBuilder.Entity<AuditEntry>().HasIndex(w => new { w.CorrelationId, w.EntityKey, w.UserId });
            modelBuilder.Entity<AuditEntry>().ToTable(auditRConfiguration.AuditEntryTableName, auditRConfiguration.Schema);

            modelBuilder.Entity<AuditEntryProperty>(b =>
            {
                b.Property(c => c.NewValue).IsRequired();
                b.Property(c => c.OldValue).IsRequired();
                b.Property(c => c.PropertyName).IsRequired();
            });
            modelBuilder.Entity<AuditEntryProperty>().ToTable(auditRConfiguration.AuditEntryPropertyTableName, auditRConfiguration.Schema);

            modelBuilder.Entity<AuditEntry>().HasMany(m => m.AuditEntryProperties).WithOne(m => m.AuditEntry).IsRequired();
            return modelBuilder;
        }
    }
}
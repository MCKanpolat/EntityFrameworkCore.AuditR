using System;
using EntityFrameworkCore.AuditR.Exceptions;
using EntityFrameworkCore.AuditR.Extensions;
using EntityFrameworkCore.AuditR.Interceptors;
using EntityFrameworkCore.AuditR.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.AuditR;

public class AuditRDbContext : DbContext
{
    private readonly AuditRConfiguration _auditRConfiguration;
    private readonly IServiceProvider _serviceProvider;

    public AuditRDbContext(IServiceProvider serviceProvider, DbContextOptions options) : base(options)
    {
        _serviceProvider = serviceProvider;
        var configuration= _serviceProvider.GetService<AuditRConfiguration>();
        _auditRConfiguration = configuration ?? throw new AuditRNotConfiguredException();
    }

    public virtual DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();
    public virtual DbSet<AuditEntryProperty> AuditEntryProperties => Set<AuditEntryProperty>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.MapAuditR(_auditRConfiguration);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditInterceptor(_serviceProvider));
        base.OnConfiguring(optionsBuilder);
    }
}
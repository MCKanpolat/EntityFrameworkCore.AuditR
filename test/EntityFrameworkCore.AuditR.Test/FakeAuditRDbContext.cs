using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.AuditR.Test;

public class FakeAuditRDbContext : AuditRDbContext
{
    public FakeAuditRDbContext(IServiceProvider serviceProvider, DbContextOptions options)
        : base(serviceProvider, options)
    {
    }

    public virtual DbSet<FakeDbModel> FakeDbModels { get; set; }
    public virtual DbSet<FakeDbModelWithValueResolver> FakeDbModelWithValueResolvers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
        }
            
        base.OnConfiguring(optionsBuilder);
    }
}
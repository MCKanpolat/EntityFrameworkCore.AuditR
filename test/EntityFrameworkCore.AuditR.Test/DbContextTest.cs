using System;
using System.Linq;
using AutoFixture.Xunit2;
using EntityFrameworkCore.AuditR.Abstraction;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EntityFrameworkCore.AuditR.Test;

public class DbContextTest
{
    private readonly AuditRConfiguration _auditRConfiguration = new();

    private readonly AuditUser _auditUser = new() { Id = "10", Name = "FakeUser", IpAddress = "12.34.56.78" };

    [Theory, AutoData]
    public void Db_Insert_Audit_Without_EntryProperties(FakeDbModel fakeDbModel)
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
            .UseSqlite(connection)
            .Options;

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(w => w.GetService(typeof(ICurrentUser))).Returns(_auditUser);
        serviceProvider.Setup(w => w.GetService(typeof(AuditRConfiguration))).Returns(_auditRConfiguration);


        using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            context.Database.EnsureCreated();
        }

        using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            context.FakeDbModels.Add(fakeDbModel);
            context.SaveChanges();
        }

        using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            Assert.True(context.FakeDbModels.Any());
            Assert.True(context.AuditEntries.Any());
            Assert.False(context.AuditEntryProperties.Any());
        }
    }

    [Theory, AutoData]
    public void Db_Insert_Audit(FakeDbModel fakeDbModel)
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
            .UseSqlite(connection)
            .Options;

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(w => w.GetService(typeof(ICurrentUser))).Returns(_auditUser);
        serviceProvider.Setup(w => w.GetService(typeof(AuditRConfiguration))).Returns(_auditRConfiguration);


        using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            context.Database.EnsureCreated();
            context.FakeDbModels.Add(fakeDbModel);
            context.SaveChanges();
        }

        using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            Assert.True(context.FakeDbModels.Any());
            Assert.True(context.AuditEntries.Any());
        }
    }

    [Theory, AutoData]
    public void Db_Update_Audit(FakeDbModel fakeDbModel)
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
            .UseSqlite(connection)
            .Options;

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(w => w.GetService(typeof(ICurrentUser))).Returns(_auditUser);
        serviceProvider.Setup(w => w.GetService(typeof(AuditRConfiguration))).Returns(_auditRConfiguration);


        using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            context.Database.EnsureCreated();

            context.FakeDbModels.Add(fakeDbModel);
            context.SaveChanges();

            fakeDbModel.Name = "test_data";
            context.SaveChanges();
        }

        using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            Assert.True(context.FakeDbModels.Any());
            Assert.Equal(2, context.AuditEntries.Count());
            Assert.Equal(1, context.AuditEntryProperties.Count());
        }
    }

    [Theory, AutoData]
    public void Db_Delete_Audit(FakeDbModel fakeDbModel)
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
            .UseSqlite(connection)
            .Options;

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(w => w.GetService(typeof(ICurrentUser))).Returns(_auditUser);
        serviceProvider.Setup(w => w.GetService(typeof(AuditRConfiguration))).Returns(_auditRConfiguration);

        using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            context.Database.EnsureCreated();
        }

        using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            context.FakeDbModels.Add(fakeDbModel);
            context.SaveChanges();

            context.FakeDbModels.Remove(fakeDbModel);
            context.SaveChanges();
        }

        using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            Assert.False(context.FakeDbModels.Any());
            Assert.Equal(2, context.AuditEntries.Count());
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using EntityFrameworkCore.AuditR.Abstraction;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EntityFrameworkCore.AuditR.Test;

public class DbContextAsyncTest
{
    private readonly AuditRConfiguration _auditRConfiguration = new();

    private readonly AuditUser _auditUser = new() { Id = "10", Name = "FakeUser", IpAddress = "12.34.56.78" };

    [Theory, AutoData]
    public async Task Db_Insert_Audit_Without_EntryProperties(FakeDbModel fakeDbModel)
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
            .UseSqlite(connection)
            .Options;

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(w => w.GetService(typeof(ICurrentUser))).Returns(_auditUser);
        serviceProvider.Setup(w => w.GetService(typeof(AuditRConfiguration))).Returns(_auditRConfiguration);


        await using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            _ = await context.Database.EnsureCreatedAsync();

            context.FakeDbModels.Add(fakeDbModel);
            _ = await context.SaveChangesAsync();
        }

        await using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            Assert.True(context.FakeDbModels.Any());
            Assert.True(context.AuditEntries.Any());
            Assert.False(context.AuditEntryProperties.Any());
        }
    }

    [Theory, AutoData]
    public async Task Db_Insert_Audit(FakeDbModel fakeDbModel)
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
            .UseSqlite(connection)
            .Options;

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(w => w.GetService(typeof(ICurrentUser))).Returns(_auditUser);
        serviceProvider.Setup(w => w.GetService(typeof(AuditRConfiguration))).Returns(_auditRConfiguration);

        await using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            _ = await context.Database.EnsureCreatedAsync();
            context.FakeDbModels.Add(fakeDbModel);
            _ = await context.SaveChangesAsync();
        }

        await using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            Assert.True(context.FakeDbModels.Any());
            Assert.True(context.AuditEntries.Any());
        }
    }

    [Theory, AutoData]
    public async Task Db_Update_Audit(FakeDbModel fakeDbModel)
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
            .UseSqlite(connection)
            .Options;

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(w => w.GetService(typeof(ICurrentUser))).Returns(_auditUser);
        serviceProvider.Setup(w => w.GetService(typeof(AuditRConfiguration))).Returns(_auditRConfiguration);


        await using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            _ = await context.Database.EnsureCreatedAsync();

            context.FakeDbModels.Add(fakeDbModel);
            _ = await context.SaveChangesAsync();

            fakeDbModel.Name = "test_data";
            _ = await context.SaveChangesAsync();
        }

        await using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            Assert.True(context.FakeDbModels.Any());
            Assert.Equal(2, context.AuditEntries.Count());
            Assert.Equal(1, context.AuditEntryProperties.Count());
        }
    }

    [Theory, AutoData]
    public async Task Db_Delete_Audit(FakeDbModel fakeDbModel)
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
            .UseSqlite(connection)
            .Options;

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(w => w.GetService(typeof(ICurrentUser))).Returns(_auditUser);
        serviceProvider.Setup(w => w.GetService(typeof(AuditRConfiguration))).Returns(_auditRConfiguration);

        await using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            _ = await context.Database.EnsureCreatedAsync();

            context.FakeDbModels.Add(fakeDbModel);
            _ = await context.SaveChangesAsync();

            context.FakeDbModels.Remove(fakeDbModel);
            _ = await context.SaveChangesAsync();
        }

        await using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            Assert.False(context.FakeDbModels.Any());
            Assert.Equal(2, context.AuditEntries.Count());
        }
    }
}
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

public class AuditValueResolverTest
{
    private readonly AuditRConfiguration _auditRConfiguration = new();

    private readonly AuditUser _auditUser = new() { Id = "10", Name = "FakeUser", IpAddress = "12.34.56.78" };

    [Theory, AutoData]
    public async Task Db_Update_Audit_Resolver(FakeDbModelWithValueResolver fakeDbModel)
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
            .UseSqlite(connection)
            .Options;
        const string resolverValue = "test_data";

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(w => w.GetService(typeof(ICurrentUser))).Returns(_auditUser);
        serviceProvider.Setup(w => w.GetService(typeof(AuditRConfiguration))).Returns(_auditRConfiguration);
        serviceProvider.Setup(w => w.GetService(typeof(CustomAuditValueResolver)))
            .Returns(new CustomAuditValueResolver(resolverValue));


        await using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            _ = await context.Database.EnsureCreatedAsync();

            context.FakeDbModelWithValueResolvers.Add(fakeDbModel);
            _ = await context.SaveChangesAsync();

            fakeDbModel.Name = "test_data";
            fakeDbModel.TestInt += 1;
            _ = await context.SaveChangesAsync();
        }

        await using (var context = new FakeAuditRDbContext(serviceProvider.Object, options))
        {
            Assert.True(context.FakeDbModelWithValueResolvers.Any());
            Assert.Equal(2, context.AuditEntries.Count());
            Assert.Equal(2, context.AuditEntryProperties.Count());

            foreach (var auditEntryProperty in context.AuditEntryProperties.Where(w =>
                         w.PropertyName == nameof(FakeDbModelWithValueResolver.TestInt)))
            {
                Assert.Equal(resolverValue, auditEntryProperty.NewValue);
                Assert.Equal(resolverValue, auditEntryProperty.OldValue);
            }
        }
    }
}
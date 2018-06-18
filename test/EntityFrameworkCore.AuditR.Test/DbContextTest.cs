using System.Data.SQLite;
using System.Linq;
using AutoFixture.Xunit2;
using EntityFrameworkCore.AuditR.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFrameworkCore.AuditR.Test
{

    public class DbContextTest
    {
        private readonly AuditRConfiguration _auditRConfiguration = new AuditRConfiguration(keyDefaultValue: KeyDefaultValue.None);
        private readonly AuditUser _auditUser = new AuditUser { Id = 10, Name = "FakeUser" };

        [Theory, AutoData]
        public void Db_Insert_Audit(FakeDbModel fakeDbModel)
        {
            var connection = new SQLiteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
             .UseSqlite(connection)
             .Options;

            try
            {
                using (var context = new FakeAuditRDbContext(() => _auditUser, _auditRConfiguration, options))
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new FakeAuditRDbContext(() => _auditUser, _auditRConfiguration, options))
                {
                    context.FakeDbModels.Add(fakeDbModel);
                    context.SaveChanges();
                }

                using (var context = new FakeAuditRDbContext(() => _auditUser, _auditRConfiguration, options))
                {
                    Assert.True(context.FakeDbModels.Any());
                    Assert.True(context.AuditEntries.Any());
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Theory, AutoData]
        public void Db_Update_Audit(FakeDbModel fakeDbModel)
        {
            var connection = new SQLiteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
             .UseSqlite(connection)
             .Options;

            try
            {
                using (var context = new FakeAuditRDbContext(() => _auditUser, _auditRConfiguration, options))
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new FakeAuditRDbContext(() => _auditUser, _auditRConfiguration, options))
                {
                    context.FakeDbModels.Add(fakeDbModel);
                    context.SaveChanges();
                    fakeDbModel.Name = "test_data";
                    context.SaveChanges();
                }

                using (var context = new FakeAuditRDbContext(() => _auditUser, _auditRConfiguration, options))
                {
                    Assert.True(context.FakeDbModels.Any());
                    Assert.True(context.AuditEntryProperties.Count() == 1);
                    Assert.True(context.AuditEntries.Count() == 2);
                }
            }
            finally
            {
                connection.Close();
            }
        }


        [Theory, AutoData]
        public void Db_Delete_Audit(FakeDbModel fakeDbModel)
        {
            var connection = new SQLiteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<FakeAuditRDbContext>()
             .UseSqlite(connection)
             .Options;

            try
            {
                using (var context = new FakeAuditRDbContext(() => _auditUser, _auditRConfiguration, options))
                {
                    context.Database.EnsureCreated();
                }

                using (var context = new FakeAuditRDbContext(() => _auditUser, _auditRConfiguration, options))
                {
                    context.FakeDbModels.Add(fakeDbModel);
                    context.SaveChanges();

                    context.FakeDbModels.Remove(fakeDbModel);
                    context.SaveChanges();
                }

                using (var context = new FakeAuditRDbContext(() => _auditUser, _auditRConfiguration, options))
                {
                    Assert.True(context.FakeDbModels.Count() == 0);
                    Assert.True(context.AuditEntries.Count() == 2);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
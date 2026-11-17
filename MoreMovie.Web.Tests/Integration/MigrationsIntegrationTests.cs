using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MoreMovies.Data;
using System.Linq;
using Xunit;

namespace MoreMovie.Web.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class MigrationsIntegrationTests
    {
        [Fact]
        public void ExistingMigrationsGenerateSqlForBothDirections()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer()
                .Options;
            using var db = new ApplicationDbContext(options);
            var migrator = db.GetService<IMigrator>();
            var latestMigration = db.Database.GetMigrations().Last();

            var upgrade = migrator.GenerateScript();
            var rollback = migrator.GenerateScript(latestMigration, Migration.InitialDatabase);

            Assert.Contains("CREATE TABLE [Movies]", upgrade);
            Assert.Contains("DROP TABLE [Movies]", rollback);
            Assert.False(db.Database.HasPendingModelChanges());
        }
    }
}

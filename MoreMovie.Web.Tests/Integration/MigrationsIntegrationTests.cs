using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using System;
using Xunit;

namespace MoreMovie.Web.Tests.Integration
{
    // Requires a reachable SQL Server instance (uses the real ApplicationDbContext
    // connection string) - kept separate from the in-memory unit test classes.
    [Trait("Category", "Integration")]
    public class MigrationsIntegrationTests
    {
        [Fact]
        public void MigrationsUpDownTest()
        {
            // Unit tests don't have a DataDirectory by default to store DB in
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());

            // Drop and recreate database
            ApplicationDbContext db = new ApplicationDbContext();
            db.Database.EnsureDeleted();

            // Retrieve migrations
            db.Database.Migrate();

            // Optional: delete database
            db.Database.EnsureDeleted();
        }
    }
}

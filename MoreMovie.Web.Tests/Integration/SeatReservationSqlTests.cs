using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests.Integration;

public sealed class SeatReservationSqlTests
{
    private const string ConnectionVariable = "MOREMOVIES_SQL_TEST_CONNECTION";

    /// <summary>Verify concurrent last-seat requests, ownership, expiry and atomic group holds.</summary>
    [SqlSeatTest]
    public async Task OnlyOneCustomerCanHoldTheLastSeat()
    {
        var connection = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable(ConnectionVariable))
        {
            InitialCatalog = $"MoreMoviesSeatTest_{Guid.NewGuid():N}"
        };
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connection.ConnectionString).Options;
        await using var db = new ApplicationDbContext(options);
        try
        {
            await db.Database.MigrateAsync();
            var screening = new CinemaPojection
            {
                Cinema = new Cinema
                {
                    Name = "Test cinema",
                    User = new IdentityUser { UserName = "test-owner" }
                },
                CinemaHall = new CinemaHall { Name = "Test hall" },
                MovieName = "Last seat",
                ProjectionTime = DateTime.Today.AddDays(1),
                Time = "18:00",
                SeatRows = 1,
                SeatsPerRow = 1
            };
            db.CinemaPojections.Add(screening);
            await db.SaveChangesAsync();

            await using var firstDb = new ApplicationDbContext(options);
            await using var secondDb = new ApplicationDbContext(options);
            var first = new SeatReservationService(firstDb, TimeProvider.System);
            var second = new SeatReservationService(secondDb, TimeProvider.System);
            var results = await Task.WhenAll(
                first.ReserveAsync(screening.Id, "first", [1]),
                second.ReserveAsync(screening.Id, "second", [1]));
            Assert.Single(results, result => result == SeatReservationService.ReservationResult.Held);
            Assert.Single(results, result => result == SeatReservationService.ReservationResult.Unavailable);

            var winner = results[0] == SeatReservationService.ReservationResult.Held ? "first" : "second";
            var other = winner == "first" ? "second" : "first";
            var reservation = await db.ScreeningSeatHolds.AsNoTracking().SingleAsync();
            Assert.Equal(winner, reservation.UserId);

            var service = new SeatReservationService(db, TimeProvider.System);
            await service.ReleaseAsync(screening.Id, reservation.ReservationId, other);
            Assert.Equal(1, await db.ScreeningSeatHolds.CountAsync());
            await db.ScreeningSeatHolds.ExecuteUpdateAsync(update =>
                update.SetProperty(hold => hold.ExpiresAt, DateTimeOffset.UtcNow.AddMinutes(-1)));
            Assert.Equal(SeatReservationService.ReservationResult.Held,
                await service.ReserveAsync(screening.Id, other, [1]));

            await db.CinemaPojections.Where(show => show.Id == screening.Id)
                .ExecuteUpdateAsync(update => update.SetProperty(show => show.SeatsPerRow, 3));
            await using var groupDb = new ApplicationDbContext(options);
            var group = new SeatReservationService(groupDb, TimeProvider.System);
            Assert.Equal(SeatReservationService.ReservationResult.Unavailable,
                await group.ReserveAsync(screening.Id, winner, [1, 2]));
            Assert.Equal(1, await db.ScreeningSeatHolds.CountAsync());
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }

    private sealed class SqlSeatTestAttribute : FactAttribute
    {
        /// <summary>Skip the test when no SQL Server test connection is configured.</summary>
        public SqlSeatTestAttribute()
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ConnectionVariable)))
            {
                Skip = $"Set {ConnectionVariable} to a test SQL Server with database creation access.";
            }
        }
    }
}

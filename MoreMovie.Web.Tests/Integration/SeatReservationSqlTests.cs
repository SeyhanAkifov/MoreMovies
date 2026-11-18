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
using ConfirmationResult = MoreMovies.Services.SeatReservationService.ConfirmationResult;
using ReservationResult = MoreMovies.Services.SeatReservationService.ReservationResult;

namespace MoreMovie.Web.Tests.Integration;

public sealed class SeatReservationSqlTests
{
    private const string ConnectionVariable = "MOREMOVIES_SQL_TEST_CONNECTION";

    /// <summary>Verify concurrent holds, atomic groups and test ticket confirmation on SQL Server.</summary>
    [SqlSeatTest]
    public async Task OnlyOneCustomerCanHoldTheLastSeat()
    {
        var options = CreateDatabaseOptions();
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
            Assert.Equal(ConfirmationResult.Unavailable, await service.ConfirmAsync(screening.Id, reservation.ReservationId, winner));
            Assert.Equal(SeatReservationService.ReservationResult.Held,
                await service.ReserveAsync(screening.Id, other, [1]));

            await db.CinemaPojections.Where(show => show.Id == screening.Id)
                .ExecuteUpdateAsync(update => update.SetProperty(show => show.SeatsPerRow, 3));
            await using var groupDb = new ApplicationDbContext(options);
            var group = new SeatReservationService(groupDb, TimeProvider.System);
            Assert.Equal(SeatReservationService.ReservationResult.Unavailable,
                await group.ReserveAsync(screening.Id, winner, [1, 2]));
            Assert.Equal(1, await db.ScreeningSeatHolds.CountAsync());

            var validHold = await db.ScreeningSeatHolds.AsNoTracking().SingleAsync();
            Assert.Equal(ConfirmationResult.Unavailable, await service.ConfirmAsync(screening.Id, validHold.ReservationId, winner));
            Assert.Equal(ConfirmationResult.Confirmed, await service.ConfirmAsync(screening.Id, validHold.ReservationId, other));
            Assert.Equal(ConfirmationResult.Confirmed, await service.ConfirmAsync(screening.Id, validHold.ReservationId, other));
            await service.ReleaseAsync(screening.Id, validHold.ReservationId, other);
            Assert.Equal(1, await db.ScreeningSeatHolds.CountAsync());
            await db.ScreeningSeatHolds.ExecuteUpdateAsync(update =>
                update.SetProperty(hold => hold.ExpiresAt, DateTimeOffset.UtcNow.AddMinutes(-1)));
            await using var afterCheckout = new ApplicationDbContext(options);
            var retry = new SeatReservationService(afterCheckout, TimeProvider.System);
            Assert.Equal(SeatReservationService.ReservationResult.Unavailable,
                await retry.ReserveAsync(screening.Id, winner, [1]));
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }

    /// <summary>Race cancellation with a seat hold and checkout without losing booked seats.</summary>
    [SqlSeatTest]
    public async Task CancellationClosesBookingAndPreservesHistory()
    {
        var options = CreateDatabaseOptions();
        await using var db = new ApplicationDbContext(options);
        try
        {
            await db.Database.MigrateAsync();
            var owner = new IdentityUser { UserName = "cinema-owner" };
            var screening = new CinemaPojection
            {
                Cinema = new Cinema { Name = "Test cinema", User = owner },
                CinemaHall = new CinemaHall { Name = "Test hall" },
                MovieName = "Final cut",
                ProjectionTime = DateTime.Today.AddDays(1),
                Time = "20:00",
                SeatRows = 1,
                SeatsPerRow = 4
            };
            db.CinemaPojections.Add(screening);
            await db.SaveChangesAsync();

            var cinema = new CinemaService(db, null, TimeProvider.System);
            var seats = new SeatReservationService(db, TimeProvider.System);
            Assert.False(await cinema.CancelAsync(screening.Id, "other-user"));
            Assert.Equal(ReservationResult.Held, await seats.ReserveAsync(screening.Id, "booked", [1]));
            Assert.Equal(ReservationResult.Held, await seats.ReserveAsync(screening.Id, "checkout", [2]));
            Assert.Equal(ReservationResult.Held, await seats.ReserveAsync(screening.Id, "pending", [4]));
            var holds = await db.ScreeningSeatHolds.AsNoTracking().ToDictionaryAsync(hold => hold.SeatNumber);
            Assert.Equal(ConfirmationResult.Confirmed,
                await seats.ConfirmAsync(screening.Id, holds[1].ReservationId, "booked"));

            await using var cancelDb = new ApplicationDbContext(options);
            await using var reserveDb = new ApplicationDbContext(options);
            await using var confirmDb = new ApplicationDbContext(options);
            var manager = new CinemaService(cancelDb, null, TimeProvider.System);
            var checkout = new SeatReservationService(confirmDb, TimeProvider.System);
            var buyer = new SeatReservationService(reserveDb, TimeProvider.System);

            var cancel = manager.CancelAsync(screening.Id, owner.Id);
            var confirm = checkout.ConfirmAsync(screening.Id, holds[2].ReservationId, "checkout");
            var reserve = buyer.ReserveAsync(screening.Id, "late", [3]);
            await Task.WhenAll(cancel, confirm, reserve);
            Assert.True(await cancel);
            var confirmation = await confirm;
            Assert.Contains(confirmation, new[] { ConfirmationResult.Confirmed, ConfirmationResult.Cancelled });
            Assert.Contains(await reserve, new[] { ReservationResult.Held, ReservationResult.Cancelled });

            var retained = await db.ScreeningSeatHolds.AsNoTracking().OrderBy(hold => hold.SeatNumber).ToListAsync();
            Assert.Equal(confirmation == ConfirmationResult.Confirmed ? new[] { 1, 2 } : new[] { 1 },
                retained.Select(hold => hold.SeatNumber));
            Assert.All(retained, hold => Assert.NotNull(hold.ConfirmedAt));
            Assert.Equal(holds[1].ReservationId, retained[0].ReservationId);

            var cancelledAt = await db.CinemaPojections.Where(show => show.Id == screening.Id)
                .Select(show => show.CancelledAt).SingleAsync();
            Assert.NotNull(cancelledAt);
            Assert.True(await cinema.CancelAsync(screening.Id, owner.Id));
            Assert.Equal(cancelledAt, await db.CinemaPojections.Where(show => show.Id == screening.Id)
                .Select(show => show.CancelledAt).SingleAsync());
            Assert.True(Assert.Single(cinema.GetAll(screening.Cinema.Name)).Cancelled);
            Assert.Equal(ReservationResult.Cancelled, await seats.ReserveAsync(screening.Id, "late", [3]));
            Assert.Equal(ConfirmationResult.Cancelled,
                await seats.ConfirmAsync(screening.Id, holds[4].ReservationId, "pending"));

            var plan = await seats.GetPlanAsync(screening.Id, "booked");
            Assert.True(plan.Cancelled);
            Assert.False(plan.Open);
            Assert.Equal(holds[1].ReservationId, Assert.Single(plan.Seats, seat => seat.Confirmed).OwnReservation);
            Assert.DoesNotContain((await seats.GetPlanAsync(screening.Id, "pending")).Seats,
                seat => seat.OwnReservation.HasValue);
        }
        finally
        {
            await db.Database.EnsureDeletedAsync();
        }
    }

    /// <summary>Give each SQL test an independent disposable database.</summary>
    private static DbContextOptions<ApplicationDbContext> CreateDatabaseOptions()
    {
        var connection = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable(ConnectionVariable))
        {
            InitialCatalog = $"MoreMoviesSeatTest_{Guid.NewGuid():N}"
        };
        return new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(connection.ConnectionString).Options;
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

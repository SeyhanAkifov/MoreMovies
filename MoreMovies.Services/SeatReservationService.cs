using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Services;

public class SeatReservationService(ApplicationDbContext db, TimeProvider clock)
{
    public record Seat(int Number, Guid? OwnReservation, DateTimeOffset? OwnExpiry, bool Unavailable, bool Confirmed);
    public record SeatPlan(string Movie, DateTime Date, string Time, int Rows, int SeatsPerRow, bool Open, IReadOnlyList<Seat> Seats);
    public enum ReservationResult { Held, Unavailable, Invalid, Missing, Closed }

    /// <summary>Read the published seating layout and current live holds.</summary>
    public async Task<SeatPlan> GetPlanAsync(int screeningId, string userId)
    {
        var screening = await db.CinemaPojections.AsNoTracking()
            .FirstOrDefaultAsync(show => show.Id == screeningId && show.SeatRows > 0 && show.SeatsPerRow > 0);
        if (screening is null)
        {
            return null;
        }

        var now = clock.GetUtcNow();
        var held = await db.ScreeningSeatHolds.AsNoTracking()
            .Where(seat => seat.ScreeningId == screeningId && (seat.ConfirmedAt != null || seat.ExpiresAt > now))
            .ToDictionaryAsync(seat => seat.SeatNumber);
        var seats = Enumerable.Range(1, screening.SeatRows * screening.SeatsPerRow).Select(number =>
        {
            held.TryGetValue(number, out var hold);
            var owned = hold is not null && hold.UserId == userId;
            return new Seat(number, owned ? hold.ReservationId : null, owned ? hold.ExpiresAt : null,
                hold is not null, owned && hold.ConfirmedAt.HasValue);
        }).ToArray();

        return new SeatPlan(screening.MovieName, screening.ProjectionTime, screening.Time,
            screening.SeatRows, screening.SeatsPerRow, CanReserve(screening), seats);
    }

    /// <summary>Hold every requested seat for ten minutes or leave none held.</summary>
    public async Task<ReservationResult> ReserveAsync(int screeningId, string userId, int[] seats)
    {
        var screening = await db.CinemaPojections.AsNoTracking().SingleOrDefaultAsync(show => show.Id == screeningId);
        if (screening is null || screening.SeatRows == 0 || screening.SeatsPerRow == 0)
        {
            return ReservationResult.Missing;
        }

        if (!CanReserve(screening))
        {
            return ReservationResult.Closed;
        }

        var capacity = screening.SeatRows * screening.SeatsPerRow;
        if (seats is null || seats.Length is < 1 or > 8 ||
            seats.Distinct().Count() != seats.Length || seats.Any(seat => seat < 1 || seat > capacity))
        {
            return ReservationResult.Invalid;
        }

        var now = clock.GetUtcNow();
        var reservationId = Guid.NewGuid();
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            await db.ScreeningSeatHolds
                .Where(hold => hold.ScreeningId == screeningId && hold.ConfirmedAt == null &&
                    hold.ExpiresAt <= now && seats.Contains(hold.SeatNumber))
                .ExecuteDeleteAsync();

            db.ScreeningSeatHolds.AddRange(seats.OrderBy(number => number).Select(number => new ScreeningSeatHold
            {
                ScreeningId = screeningId,
                SeatNumber = number,
                ReservationId = reservationId,
                UserId = userId,
                ExpiresAt = now.AddMinutes(10)
            }));
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return ReservationResult.Held;
        }
        catch (DbUpdateException error) when (error.InnerException is SqlException { Number: 2601 or 2627 or 1205 })
        {
            return ReservationResult.Unavailable;
        }
        catch (SqlException error) when (error.Number == 1205)
        {
            return ReservationResult.Unavailable;
        }
    }

    /// <summary>Release the signed-in user's hold for the selected screening.</summary>
    public async Task ReleaseAsync(int screeningId, Guid reservationId, string userId)
    {
        await db.ScreeningSeatHolds.Where(hold => hold.ScreeningId == screeningId &&
            hold.ReservationId == reservationId && hold.UserId == userId && hold.ConfirmedAt == null)
            .ExecuteDeleteAsync();
    }

    /// <summary>Confirm all seats in a valid hold as a single test booking.</summary>
    public async Task<bool> ConfirmAsync(int screeningId, Guid reservationId, string userId)
    {
        var now = clock.GetUtcNow();
        var reservation = db.ScreeningSeatHolds.Where(hold => hold.ScreeningId == screeningId &&
            hold.ReservationId == reservationId && hold.UserId == userId);
        var completed = await reservation.Where(hold => hold.ConfirmedAt == null && hold.ExpiresAt > now)
            .ExecuteUpdateAsync(update => update.SetProperty(hold => hold.ConfirmedAt, now));
        return completed > 0 || await reservation.AnyAsync(hold => hold.ConfirmedAt != null);
    }

    /// <summary>Check whether the local screening start is still in the future.</summary>
    private bool CanReserve(CinemaPojection screening) =>
        TimeOnly.TryParseExact(screening.Time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var time) &&
        screening.ProjectionTime.Date.Add(time.ToTimeSpan()) > clock.GetLocalNow().DateTime;
}

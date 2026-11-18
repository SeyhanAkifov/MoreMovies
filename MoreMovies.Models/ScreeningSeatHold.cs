using System;

namespace MoreMovies.Models;

public class ScreeningSeatHold
{
    public int ScreeningId { get; set; }

    public int SeatNumber { get; set; }

    public Guid ReservationId { get; set; }

    public string UserId { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? ConfirmedAt { get; set; }
}

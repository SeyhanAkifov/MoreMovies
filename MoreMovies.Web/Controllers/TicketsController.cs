using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MoreMovies.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers;

[Authorize]
public class TicketsController(SeatReservationService reservations, IWebHostEnvironment environment) : Controller
{
    /// <summary>Show seat availability and the signed-in user's reservations.</summary>
    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Seats(int id)
    {
        var plan = await reservations.GetPlanAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (plan is null)
        {
            return NotFound();
        }

        ViewData["ScreeningId"] = id;
        ViewData["TestCheckout"] = environment.IsDevelopment();
        return View(plan);
    }

    /// <summary>Reserve the selected seats and report conflicts to the seat map.</summary>
    [HttpPost]
    public async Task<IActionResult> Reserve(int id, int[] seats)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await reservations.ReserveAsync(id, userId, seats);
        var wantsJson = Request.GetTypedHeaders().Accept?.Any(type => type.MediaType.Value == "application/json") == true;
        if (result == SeatReservationService.ReservationResult.Missing)
        {
            return NotFound();
        }

        if (result == SeatReservationService.ReservationResult.Invalid)
        {
            return BadRequest("Select between one and eight valid seats.");
        }

        if (result == SeatReservationService.ReservationResult.Closed)
        {
            return Conflict("Reservations for this screening are closed.");
        }

        if (result is SeatReservationService.ReservationResult.Unavailable or SeatReservationService.ReservationResult.Cancelled)
        {
            if (wantsJson)
            {
                var plan = result == SeatReservationService.ReservationResult.Unavailable
                    ? await reservations.GetPlanAsync(id, userId)
                    : null;
                if (result == SeatReservationService.ReservationResult.Cancelled || plan?.Cancelled == true)
                {
                    return Conflict(new { url = Url.Action(nameof(Seats), new { id }) });
                }

                return Conflict(new
                {
                    unavailable = plan?.Seats.Where(seat => seat.Unavailable).Select(seat => seat.Number)
                });
            }

            if (result == SeatReservationService.ReservationResult.Unavailable)
            {
                TempData["SeatError"] = "Availability changed. Please select your seats again.";
            }

            return RedirectToAction(nameof(Seats), new { id });
        }

        TempData["SeatSuccess"] = "Seats reserved for 10 minutes.";
        return wantsJson ? Json(new { url = Url.Action(nameof(Seats), new { id }) }) :
            RedirectToAction(nameof(Seats), new { id });
    }

    /// <summary>Release a reservation belonging to the signed-in user.</summary>
    [HttpPost]
    public async Task<IActionResult> Release(int id, Guid reservationId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        await reservations.ReleaseAsync(id, reservationId, User.FindFirstValue(ClaimTypes.NameIdentifier));
        return RedirectToAction(nameof(Seats), new { id });
    }

    /// <summary>Complete a test booking without payment in development.</summary>
    [HttpPost]
    public async Task<IActionResult> Confirm(int id, Guid reservationId)
    {
        if (!environment.IsDevelopment())
        {
            return NotFound();
        }

        if (!ModelState.IsValid || reservationId == Guid.Empty)
        {
            return BadRequest();
        }

        var result = await reservations.ConfirmAsync(id, reservationId, User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (result == SeatReservationService.ConfirmationResult.Unavailable)
        {
            TempData["SeatError"] = "This reservation has expired or is unavailable.";
        }
        else if (result == SeatReservationService.ConfirmationResult.Confirmed)
        {
            TempData["SeatSuccess"] = "Test booking confirmed. No payment was collected.";
        }

        return RedirectToAction(nameof(Seats), new { id });
    }
}

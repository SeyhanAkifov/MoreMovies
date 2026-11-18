using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers;

[Authorize]
public class TicketsController(SeatReservationService reservations) : Controller
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

        if (result == SeatReservationService.ReservationResult.Unavailable)
        {
            if (wantsJson)
            {
                var plan = await reservations.GetPlanAsync(id, userId);
                return Conflict(new
                {
                    unavailable = plan?.Seats.Where(seat => seat.Unavailable).Select(seat => seat.Number)
                });
            }

            TempData["SeatError"] = "Availability changed. Please select your seats again.";
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
}

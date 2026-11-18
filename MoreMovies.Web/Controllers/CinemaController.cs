using System;
using System.Globalization;
using System.Linq;
using MoreMovies.Web.MappingConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models.Cinema;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class CinemaController : Controller
    {
        private readonly ICinemaService cinemaService;

        public CinemaController(ICinemaService cinemaService)
        {
            this.cinemaService = cinemaService;
        }

        /// <summary>Show seven days of screenings from the selected date.</summary>
        [HttpGet]
        public IActionResult InCinema(string cinemaName, DateOnly? start)
        {

            var cinemaNames = this.cinemaService.GetCinemaNames();
            var selectedCinema = cinemaNames.Contains(cinemaName) ? cinemaName : cinemaNames.FirstOrDefault();
            var projections = cinemaService.GetAll(selectedCinema, User.FindFirstValue(ClaimTypes.NameIdentifier));


            var result = (projections ?? System.Array.Empty<CinemaProjectionOutputDto>()).Select(ViewModelMappings.ToViewModel).ToList();

            var d = new InCinemaViewModel
            {
                CinemaNames = cinemaNames,
                SelectedCinema = selectedCinema,
                StartDate = start is { Year: >= 1900 and <= 9998 }
                    ? start.Value
                    : DateOnly.FromDateTime(DateTime.Today),
                Projections = result,
            };

            return View(d);
        }

        [HttpGet]
        public IActionResult Become()
        {
            return View();
        }



        /// <summary>Register a cinema for the signed-in user.</summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCinema(CinemaInputDto model)
        {
            if (!ModelState.IsValid)
            {
                return View("Become", model);
            }

            await cinemaService.AddCinema(model.CinemaName.Trim(), User.Identity.Name);
            return RedirectToAction(nameof(Add));
        }

        /// <summary>Show the screening form with today's date selected.</summary>
        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            var user = User.Identity.Name;

            var cinema = this.cinemaService.GetCinema(user);

            ViewBag.Names = cinema;

            return View(new CinemaProjectionInputDto { ProjectionTime = DateTime.Today });
        }

        /// <summary>Save a screening for a cinema owned by the signed-in user.</summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CinemaProjectionInputDto model)
        {
            if ((model.SeatRows == 0) != (model.SeatsPerRow == 0))
            {
                ModelState.AddModelError(nameof(model.SeatsPerRow), "Enter both seat-plan dimensions or leave both at zero.");
            }

            if (model.ProjectionTime.Year is < 1900 or > 9998)
            {
                ModelState.AddModelError(nameof(model.ProjectionTime), "Choose a valid screening date.");
            }

            if (!TimeOnly.TryParseExact(model.Time, "HH:mm", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out _))
            {
                ModelState.AddModelError(nameof(model.Time), "Choose a valid screening time.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Names = cinemaService.GetCinema(User.Identity.Name);
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await cinemaService.Add(model, userId))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(InCinema), new
            {
                cinemaName = model.CinemaName,
                start = model.ProjectionTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            });
        }

        /// <summary>Cancel a screening owned by the signed-in user.</summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string cinemaName, string start)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await cinemaService.CancelAsync(id, userId))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(InCinema), new { cinemaName, start });
        }
    }
}

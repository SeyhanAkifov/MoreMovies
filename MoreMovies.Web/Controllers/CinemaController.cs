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

        /// <summary>Show the selected cinema and its scheduled screenings.</summary>
        [HttpGet]
        public IActionResult InCinema(string cinemaName)
        {

            var cinemaNames = this.cinemaService.GetCinemaNames();
            var selectedCinema = cinemaNames.Contains(cinemaName) ? cinemaName : cinemaNames.FirstOrDefault();
            var projections = cinemaService.GetAll(selectedCinema, User.FindFirstValue(ClaimTypes.NameIdentifier));


            var result = (projections ?? System.Array.Empty<CinemaProjectionOutputDto>()).Select(ViewModelMappings.ToViewModel).ToList();

            var d = new InCinemaViewModel
            {
                CinemaNames = cinemaNames,
                SelectedCinema = selectedCinema,
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

        [Authorize]
        [HttpGet]
        public IActionResult Add()
        {
            var user = User.Identity.Name;

            var cinema = this.cinemaService.GetCinema(user);

            ViewBag.Names = cinema;

            return View();
        }

        /// <summary>Save a screening for a cinema owned by the signed-in user.</summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CinemaProjectionInputDto model)
        {
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

            return RedirectToAction(nameof(InCinema), new { cinemaName = model.CinemaName });
        }

        /// <summary>Remove a screening owned by the signed-in user.</summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string cinemaName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await cinemaService.Delete(id, userId))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(InCinema), new { cinemaName });
        }
    }
}

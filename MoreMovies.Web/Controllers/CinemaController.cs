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

        [HttpGet]
        public IActionResult InCinema(string cinemaName)
        {

            var cinemaNames = this.cinemaService.GetCinemaNames();
            var projections = this.cinemaService.GetAll(cinemaName);


            var result = (projections ?? System.Array.Empty<CinemaProjectionOutputDto>()).Select(ViewModelMappings.ToViewModel).ToList();

            var d = new InCinemaViewModel
            {
                CinemaNames = cinemaNames,
                Projections = result,
            };

            ViewBag.Name = cinemaName;

            return View(d);
        }

        [HttpGet]
        public IActionResult Become()
        {
            return View();
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCinema(string cinemaName)
        {
            var user = User.Identity.Name;



            await this.cinemaService.AddCinema(cinemaName, user);

            return RedirectToAction("InCinema");
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

            return RedirectToAction("InCinema");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!await cinemaService.Delete(id, userId))
            {
                return NotFound();
            }

            return RedirectToAction("InCinema");
        }
    }
}

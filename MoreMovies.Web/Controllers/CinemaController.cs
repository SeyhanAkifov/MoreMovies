using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models.Cinema;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class CinemaController : Controller
    {
        private readonly ICinemaService cinemaService;
        private readonly IMapper mapper;

        public CinemaController(ICinemaService cinemaService, IMapper mapper)
        {
            this.cinemaService = cinemaService;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult InCinema(string cinemaName)
        {


            var cinemaNames = this.cinemaService.GetCinemaNames();
            var projections = this.cinemaService.GetAll(cinemaName);


            var result = mapper.Map<ICollection<CinemaProjectionOutputDto>, ICollection<CinemaProjectionViewModel>>(projections);

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



        [HttpPost]
        public async Task<IActionResult> AddCinema(string cinemaName)
        {
            var user = User.Identity.Name;



            await this.cinemaService.AddCinema(cinemaName, user);

            return RedirectToAction("InCinema");
        }

        [HttpGet]
        public IActionResult Add()
        {
            var user = User.Identity.Name;

            var cinema = this.cinemaService.GetCinema(user);

            ViewBag.Names = cinema;

            return View();
        }

        [HttpPost]
        public IActionResult Add(CinemaProjectionInputDto model)
        {
            this.cinemaService.Add(model);

            return RedirectToAction("InCinema");
        }

        public IActionResult Delete(int id)
        {
            this.cinemaService.Delete(id);

            return RedirectToAction("InCinema");
        }
    }
}

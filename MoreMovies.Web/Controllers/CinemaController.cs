using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models.Cinema;
using System.Collections.Generic;

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

        public IActionResult InCinema()
        {
            var projections = this.cinemaService.GetAll();

            var result = mapper.Map<ICollection<CinemaProjectionOutputDto>, ICollection<CinemaProjectionViewModel>>(projections);

            return View(result);
        }

        [HttpGet]
        public IActionResult Add()
        {
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

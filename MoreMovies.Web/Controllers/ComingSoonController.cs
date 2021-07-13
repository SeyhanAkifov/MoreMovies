using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Models;
using MoreMovies.Services.Dto;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class ComingSoonController : Controller
    {
        private readonly IComingSoonService comingSoonService;
        private readonly IMapper mapper;

        public ComingSoonController(IComingSoonService comingSoonService, IMapper mapper)
        {
            this.comingSoonService = comingSoonService;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View(new ComingSoonAddModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(ComingSoonAddModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await this.comingSoonService.Add(model);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> All()
        {
            var comingSoon = await this.comingSoonService.GetAll();

            var comingSoonResult = mapper.Map<ICollection<ComingSoon>, ICollection<ComingSoonViewModel>>(comingSoon);

            return this.View(comingSoonResult);
        }

        public async Task<IActionResult> Details(int id)
        {
            var comingSoon = await this.comingSoonService.GetWithId(id);
            var comingSoonResult = mapper.Map<ComingSoon, ComingSoonViewModel>(comingSoon);

            return this.View(comingSoonResult);
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using System.Collections.Generic;
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
        
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var comingSoon = await this.comingSoonService.GetAll();

            var comingSoonResult = mapper.Map<ICollection<ComingSoonOutputDto>, ICollection<ComingSoonViewModel>>(comingSoon);

            return this.View(comingSoonResult);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Invalid request for id {id}");
            }

            var comingSoon = await this.comingSoonService.GetWithId(id);
            var comingSoonResult = mapper.Map<ComingSoonOutputDto, ComingSoonViewModel>(comingSoon);

            return this.View(comingSoonResult);
        }
    }
}

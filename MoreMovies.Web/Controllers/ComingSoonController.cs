using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services.Dto;
using MoreMovies.Services.Interfaces;
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
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(ComingSoonAddModel model)
        {
            await this.comingSoonService.Add(model);

            return RedirectToAction("Index", "Home");
        }
    }
}

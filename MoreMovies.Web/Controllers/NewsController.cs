using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Models;
using MoreMovies.Services.Dto;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService newsService;
        private readonly IMapper mapper;

        public NewsController(INewsService newsService, IMapper mapper)
        {
            this.newsService = newsService;
            this.mapper = mapper;
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(NewsAddModel model)
        {
            await this.newsService.Add(model);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Delete()
        {
            return View();
        }

        public async Task<IActionResult> All()
        {
            var news = await this.newsService.GetAllNews();
            var newsResult = mapper.Map<ICollection<News>, ICollection<NewsViewModel>>(news);

            return this.View(newsResult);
        }
    }
}

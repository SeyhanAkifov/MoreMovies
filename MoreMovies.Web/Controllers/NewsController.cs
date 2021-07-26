﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Models;
using MoreMovies.Services.Dto;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models.News;
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

        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View(new NewsAddModel());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(NewsAddModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await this.newsService.Add(model);

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await newsService.Delete(id);
            return RedirectToAction("All");
        }

        public async Task<IActionResult> All()
        {
            var news = await this.newsService.GetAllNews();
            var newsResult = mapper.Map<ICollection<News>, ICollection<NewsViewModel>>(news);

            return this.View(newsResult);
        }

        public async Task<IActionResult> Details(int id)
        {
            var news = await this.newsService.GetNewsWithId(id);
            var newsResult = mapper.Map<News, NewsViewModel>(news);

            return this.View(newsResult);
        }
    }
}

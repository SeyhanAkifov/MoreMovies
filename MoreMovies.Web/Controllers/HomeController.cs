using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService service;



        public HomeController(ILogger<HomeController> logger, IMovieService service)
        {
            _logger = logger;
            this.service = service;
        }

        public IActionResult Details(int id)
        {
            var movie = service.GetMovieWithId(id);
            return this.View(movie);
        }

        public IActionResult Index()
        {
            var movies = service.GetAllMovie();
            return this.View(movies);
        }

        public IActionResult HomePage()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
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
        private readonly IMapper mapper;
        private readonly ApplicationDbContext db;


        public HomeController(ILogger<HomeController> logger, IMovieService service, IMapper mapper, ApplicationDbContext db)
        {
            _logger = logger;
            this.service = service;
            this.mapper = mapper;
            this.db = db;
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await service.GetMovieWithId(id);

            var result = mapper.Map<Movie, MovieViewModel>(movie);
            var comments = db.MovieComments
                           .Join(db.Comments, c => c.CommentId, b => b.Id, (comment, c) => c)
                           .ToList();

            result.Comments = comments;
            return this.View(result);
        }

        public async Task<IActionResult> Index()
        {
            var topcomentedMovies = await service.GetTopCommentedMovie();
            var topLikedMovies = await service.GetTopLikedMovie();
            var newestAddedMovies = await service.GetNewestAddedMovie();

            var topCommentedResult = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(topcomentedMovies);
            var topLikedResult = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(topLikedMovies);
            var newestAddedResult = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(newestAddedMovies);

            var movies = new MovieListViewModel()
            {
                TopCommented = topCommentedResult,
                TopLiked = topLikedResult,
                Newest = newestAddedResult
            };


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

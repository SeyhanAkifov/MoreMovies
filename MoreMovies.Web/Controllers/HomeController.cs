using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using MoreMovies.Web.Models.News;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService service;
        private readonly IGenreService genreService;
        private readonly INewsService newsService;
        private readonly IComingSoonService comingSoonService;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext db;


        public HomeController(ILogger<HomeController> logger, IMovieService service, IMapper mapper, ApplicationDbContext db, INewsService newsService, IComingSoonService comingSoonService, IGenreService genreService)
        {
            _logger = logger;
            this.service = service;
            this.mapper = mapper;
            this.db = db;
            this.newsService = newsService;
            this.comingSoonService = comingSoonService;
            this.genreService = genreService;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var topcomentedMovies = await service.GetTopCommentedMovie();
            var topLikedMovies = await service.GetTopLikedMovie();
            var newestAddedMovies = await service.GetNewestAddedMovie();
            var news = await newsService.GetNewsForHomePage();
            var comingSoon = await comingSoonService.GetForHomePage();

            var topCommentedResult = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(topcomentedMovies);
            var topLikedResult = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(topLikedMovies);
            var newestAddedResult = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(newestAddedMovies);
            var newsResult = mapper.Map<ICollection<News>, ICollection<NewsViewModel>>(news);
            var comingSoonResult = mapper.Map<ICollection<ComingSoon>, ICollection<ComingSoonViewModel>>(comingSoon);
            var genres = await genreService.GetGenres();

            var movies = new MovieListViewModel()
            {
                TopCommented = topCommentedResult,
                TopLiked = topLikedResult,
                Newest = newestAddedResult,
                News = newsResult,
                ComingSoon = comingSoonResult,
                Genres = genres
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
            return View();
        }
    }
}

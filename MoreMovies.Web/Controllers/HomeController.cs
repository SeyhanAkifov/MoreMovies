using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using MoreMovies.Web.Models.News;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService service;
        private readonly IGenreService genreService;
        private readonly INewsService newsService;
        private readonly IComingSoonService comingSoonService;
        private readonly IMapper mapper;

        public HomeController(IMovieService service, IMapper mapper, INewsService newsService, IComingSoonService comingSoonService, IGenreService genreService)
        {
            this.service = service;
            this.mapper = mapper;
            this.newsService = newsService;
            this.comingSoonService = comingSoonService;
            this.genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var topcomentedMovies = await service.GetTopCommentedMovie();
            var topLikedMovies = await service.GetTopLikedMovie();
            var newestAddedMovies = await service.GetNewestAddedMovie();
            var news = await newsService.GetNewsForHomePage();
            var comingSoon = await comingSoonService.GetForHomePage();

            var topCommentedResult = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(topcomentedMovies);
            var topLikedResult = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(topLikedMovies);
            var newestAddedResult = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(newestAddedMovies);
            var newsResult = mapper.Map<ICollection<NewsOutputDto>, ICollection<NewsViewModel>>(news);
            var comingSoonResult = mapper.Map<ICollection<ComingSoonOutputDto>, ICollection<ComingSoonViewModel>>(comingSoon);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}

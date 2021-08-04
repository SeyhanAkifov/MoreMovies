using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using MoreMovies.Web.Models.News;
using System;
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
        private readonly IMemoryCache cache;

        public HomeController(IMovieService service, 
            IMapper mapper, 
            INewsService newsService, 
            IComingSoonService comingSoonService, 
            IGenreService genreService, 
            IMemoryCache cache)
        {
            this.service = service;
            this.mapper = mapper;
            this.newsService = newsService;
            this.comingSoonService = comingSoonService;
            this.genreService = genreService;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            const string topcomentedMoviesCacheKey = "TopcomentedMoviesCasheKey";
            const string topLikedMoviesCacheKey = "TopLikedMoviesCasheKey";
            const string newsCacheKey = "NewsCasheKey";
            const string comingSoonCacheKey = "ComingSoonCasheKey";
            const string genresCacheKey = "GenresCasheKey";

            var topcomentedMovies = this.cache.Get<ICollection<MovieOutputDto>>(topcomentedMoviesCacheKey);
            var topLikedMovies = this.cache.Get<ICollection<MovieOutputDto>>(topLikedMoviesCacheKey);
            var news = this.cache.Get<ICollection<NewsOutputDto>>(newsCacheKey);
            var comingSoon = this.cache.Get<ICollection<ComingSoonOutputDto>>(comingSoonCacheKey);
            var genres = this.cache.Get<ICollection<MoreMovies.Models.Genre>>(genresCacheKey);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

            if (topcomentedMovies == null)
            {
                topcomentedMovies = await service.GetTopCommentedMovie();
                topLikedMovies = await service.GetTopLikedMovie();
                news = await newsService.GetNewsForHomePage();
                comingSoon = await comingSoonService.GetForHomePage();
                genres = await genreService.GetGenres();

                this.cache.Set(topcomentedMoviesCacheKey, topcomentedMovies, cacheOptions);
                this.cache.Set(topLikedMoviesCacheKey, topLikedMovies, cacheOptions);
                this.cache.Set(newsCacheKey, news, cacheOptions);
                this.cache.Set(comingSoonCacheKey, comingSoon, cacheOptions);
                this.cache.Set(genresCacheKey, genres, cacheOptions);
            }
            
            var newestAddedMovies = await service.GetNewestAddedMovie();
            
            var topCommentedResult = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(topcomentedMovies);
            var topLikedResult = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(topLikedMovies);
            var newestAddedResult = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(newestAddedMovies);
            var newsResult = mapper.Map<ICollection<NewsOutputDto>, ICollection<NewsViewModel>>(news);
            var comingSoonResult = mapper.Map<ICollection<ComingSoonOutputDto>, ICollection<ComingSoonViewModel>>(comingSoon);
            
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

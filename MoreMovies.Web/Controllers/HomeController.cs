using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using MoreMovies.Web.Models.Administration;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(
            
            IMapper mapper, 
            INewsService newsService, 
            IComingSoonService comingSoonService, 
            IGenreService genreService, 
            IMemoryCache cache,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IMovieService service)
        {
            this.service = service;
            this.mapper = mapper;
            this.newsService = newsService;
            this.comingSoonService = comingSoonService;
            this.genreService = genreService;
            this.cache = cache;
            _userManager = userManager;
            _signInManager = signInManager;
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
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View("Index", "Home");
        }

        [HttpGet]

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string email, string password, string confirmPassword)
        {
            return View();
        }

        [HttpGet]
       
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            
            return Redirect("Login");
        }
    }
}

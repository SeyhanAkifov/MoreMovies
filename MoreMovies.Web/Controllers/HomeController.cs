using System.Linq;
using MoreMovies.Web.MappingConfiguration;
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
        private readonly IMemoryCache cache;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(
            
            INewsService newsService, 
            IComingSoonService comingSoonService, 
            IGenreService genreService, 
            IMemoryCache cache,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IMovieService service)
        {
            this.service = service;
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
            const string topCommentedMoviesCacheKey = "TopCommentedMoviesCacheKey";
            const string topLikedMoviesCacheKey = "TopLikedMoviesCacheKey";
            const string newsCacheKey = "NewsCacheKey";
            const string comingSoonCacheKey = "ComingSoonCacheKey";
            const string genresCacheKey = "GenresCacheKey";

            var topcomentedMovies = this.cache.Get<ICollection<MovieOutputDto>>(topCommentedMoviesCacheKey);
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
                
                this.cache.Set(topCommentedMoviesCacheKey, topcomentedMovies, cacheOptions);
                this.cache.Set(topLikedMoviesCacheKey, topLikedMovies, cacheOptions);
                this.cache.Set(newsCacheKey, news, cacheOptions);
                this.cache.Set(comingSoonCacheKey, comingSoon, cacheOptions);
                this.cache.Set(genresCacheKey, genres, cacheOptions);
            }
            
            var newestAddedMovies = await service.GetNewestAddedMovie();
            
            var topCommentedResult = (topcomentedMovies ?? System.Array.Empty<MovieOutputDto>()).Select(ViewModelMappings.ToViewModel).ToList();
            var topLikedResult = (topLikedMovies ?? System.Array.Empty<MovieOutputDto>()).Select(ViewModelMappings.ToViewModel).ToList();
            var newestAddedResult = (newestAddedMovies ?? System.Array.Empty<MovieOutputDto>()).Select(ViewModelMappings.ToViewModel).ToList();
            var newsResult = (news ?? System.Array.Empty<NewsOutputDto>()).Select(ViewModelMappings.ToViewModel).ToList();
            var comingSoonResult = (comingSoon ?? System.Array.Empty<ComingSoonOutputDto>()).Select(ViewModelMappings.ToViewModel).ToList();
            
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
        public IActionResult Login(string returnUrl)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl });
        }

        [HttpGet]

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match.");
                return View();
            }

            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
    }
}

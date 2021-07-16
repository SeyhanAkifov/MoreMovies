using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MoreMovies.Models;
using MoreMovies.Services.Dto;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Hubs;
using MoreMovies.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class MovieController : Controller
    {

        private readonly IMovieService movieService;
        private readonly ICommentService commentService;
        private readonly IActorService actorService;
        private readonly ILanguageService languageService;
        private readonly IGenreService genreService;
        private readonly ICountryService countryService;
        private readonly IHubContext<MovieHub> movieHub;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IdentityUser user;

        public MovieController(IMovieService movieService, IMapper mapper, ICommentService commentService, IActorService actorService, IdentityUser user, ILanguageService languageService, IGenreService genreService, ICountryService countryService, IHubContext<MovieHub> movieHub)
        {
            this.movieService = movieService;
            this.mapper = mapper;
            this.commentService = commentService;
            this.actorService = actorService;
            this.user = user;
            this.languageService = languageService;
            this.genreService = genreService;
            this.countryService = countryService;
            this.movieHub = movieHub;
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            if (id != 0)
            {
                var movie = await movieService.GetMovieWithId(id);

                var result = mapper.Map<Movie, MovieViewModel>(movie);

                result.Comments = this.commentService.GetMovieComments(result.Id);

                result.Actors = this.actorService.GetMovieActors(result.Id);

                  return View(result);
            }
            else
            {
                return this.View(null);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddMovieInputModel
            {
                Languages = await languageService.GetLanguages(),
                Countries = await countryService.GetCountries(),
                Genres = await genreService.GetGenres(),
            };

            return this.View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddMovie(AddMovieInputModel model)
        {
            var languages = await languageService.GetLanguages();
            var countries = await countryService.GetCountries();
            var genres = await genreService.GetGenres();

            if (!languages.Any(x => x.Name == model.Language))
            {
                this.ModelState.AddModelError(nameof(model.Language), "Language does not exist");
            }

            if (!countries.Any(x => x.Name == model.Country))
            {
                this.ModelState.AddModelError(nameof(model.Country), "Country does not exist");
            }

            if (!genres.Any(x => x.Name == model.Genre))
            {
                this.ModelState.AddModelError(nameof(model.Genre), "Genre does not exist");
            }

            if (!ModelState.IsValid)
            {
                model.Languages = languages;
                model.Countries = countries;
                model.Genres = genres;

                return View("Add", model);
            }
            
            model.UserId = User.FindFirstValue(ClaimTypes.Email);
            await movieService.AddMovie(model);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await movieService.GetMovieWithId(id);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userEmail != movie.Creator)
            {
                return RedirectToAction("Index", "Home");
            };

           

            var result = mapper.Map<Movie, MovieViewModel>(movie);

            return this.View(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditMovie(int id, AddMovieInputModel model)
        {
            
            await movieService.EditMovieWithId(id, model);

            return RedirectToAction("Details", "Movie", new { id });
        }

        [Authorize]
        public async Task<IActionResult> MyMovies()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var movies = await movieService.GetAllMyMovie(userId);
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return View(result);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await movieService.DeleteMovie(id);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Like(int id)
        {
            await movieService.LikeMovie(id);

            return RedirectToAction("Details", "Movie", new { id });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(int id, AddCommentInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            model.UserId = userEmail;
            model.MovieId = id;
            await movieService.AddComment(model);
            var movie = await movieService.GetMovieWithId(model.MovieId);
            await this.movieHub.Clients.All.SendAsync("NewMessage",  model.UserId, movie.Title);
            return RedirectToAction("Details", "Movie", new { id = id });
        }

        [Authorize]
        public IActionResult AddComment()
        {
            return View(new AddCommentInputModel());
        }


        public async Task<IActionResult> Search(string name)
        {
            if (name == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var movieId = await movieService.SearchMovie(name);

            if (movieId != 0)
            {
                return RedirectToAction("Details", "Movie", new { id = movieId });
            }

            return RedirectToAction("Details", "Movie", 0);
        }

        public async Task<IActionResult> SearchByGenre(string genre)
        {
            var movies = await movieService.SearchMovieByGenre(genre);
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return View("All", result);
        }

        public async Task<IActionResult> All()
        {

            var movies = await movieService.GetAllMovie();
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        public async Task<IActionResult> AllNewest()
        {

            var movies = await movieService.GetNewestAddedAllMovie();
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        public async Task<IActionResult> TopCommented()
        {

            var movies = await movieService.GetTopCommentedMovie();
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        public async Task<IActionResult> AllTopCommented()
        {

            var movies = await movieService.GetTopCommentedAllMovie();
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        public async Task<IActionResult> TopLiked()
        {

            var movies = await movieService.GetTopLikedMovie();
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        public async Task<IActionResult> AllTopLiked()
        {

            var movies = await movieService.GetTopLikedAllMovie();
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }


    }
}

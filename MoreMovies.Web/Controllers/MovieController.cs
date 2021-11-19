using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Hubs;
using MoreMovies.Web.Models;
using MoreMovies.Web.Models.Movie;
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


        public MovieController(
            IMovieService movieService,
            IMapper mapper,
            ICommentService commentService,
            IActorService actorService,
            ILanguageService languageService,
            IGenreService genreService,
            ICountryService countryService,
            IHubContext<MovieHub> movieHub)
        {
            this.movieService = movieService;
            this.mapper = mapper;
            this.commentService = commentService;
            this.actorService = actorService;
            this.languageService = languageService;
            this.genreService = genreService;
            this.countryService = countryService;
            this.movieHub = movieHub;
        }

        [Authorize]
        [HttpGet]
        public IActionResult InCinema()
        {
            return this.View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id != 0)
            {
                var movie = await movieService.GetMovieWithId(id);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                movie.IsUserLiked = movieService.IsUserLiked(id, userId);

                var result = mapper.Map<MovieDetailOutputDto, MovieDetailsViewModel>(movie);

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
        public async Task<IActionResult> RateMovie(int rating, int movieId)
        {
            await movieService.Ratemovie(rating, movieId);

            return RedirectToAction("Details", "Movie", new { Id = movieId });
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
        [ValidateAntiForgeryToken]
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

            await movieService.AddMovie(model);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var movie = await movieService.GetMovieWithId(id);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail != movie.Creator)
            {
                return RedirectToAction("Index", "Home");
            };
            
            var result = mapper.Map<MovieDetailOutputDto, EditMovieInputModel>(movie);

            return this.View(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditMovie(int id, EditMovieInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            await movieService.EditMovieWithId(id, model);

            return RedirectToAction("Details", "Movie", new { id });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyMovies()
        {
            var userName = User.FindFirstValue(ClaimTypes.Email);
            var movies = await movieService.GetAllMyMovie(userName);
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return View(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyLiked()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var movies = await movieService.GetAllMyLiked(userId);
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return View(result);
        }

        [Authorize]
        
        public async Task<IActionResult> Delete(int id)
        {
            await movieService.DeleteMovie(id);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Like(int id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!movieService.IsUserLiked(id, userId))
            {
                await movieService.LikeMovie(id, userId);
            }

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
            await this.movieHub.Clients.All.SendAsync("NewMessage", model.UserId, movie.Title);

            return RedirectToAction("Details", "Movie", new { id });
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddComment()
        {
            return View(new AddCommentInputModel());
        }

        [HttpGet]
        public async Task<IActionResult> Search(string name)
        {
            if (name == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var movies = await movieService.SearchMovie(name);
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return View("All", result);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByGenre(string genre)
        {
            var movies = await movieService.SearchMovieByGenre(genre);
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return View("All", result);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByYear(string year)
        {
            var movies = await movieService.SearchMovieByYear(year);
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return View("All", result);
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {

            var movies = await movieService.GetAllMovie();
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        [HttpGet]
        public async Task<IActionResult> AllNewest()
        {

            var movies = await movieService.GetNewestAddedAllMovie();
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        [HttpGet]
        public async Task<IActionResult> TopCommented()
        {

            var movies = await movieService.GetTopCommentedMovie();
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        [HttpGet]
        public async Task<IActionResult> AllTopCommented()
        {

            var movies = await movieService.GetTopCommentedAllMovie();
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        [HttpGet]
        public async Task<IActionResult> TopLiked()
        {

            var movies = await movieService.GetTopLikedMovie();
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        [HttpGet]
        public async Task<IActionResult> AllTopLiked()
        {
            var movies = await movieService.GetTopLikedAllMovie();
            var result = mapper.Map<ICollection<MovieOutputDto>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }


    }
}

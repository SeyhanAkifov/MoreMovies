using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Models;
using MoreMovies.Services.Dto;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class MovieController : Controller
    {

        private readonly IMovieService movieService;
        private readonly ICommentService commentService;
        private readonly IActorService actorService;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IdentityUser user;
        
        public MovieController(IMovieService movieService, IMapper mapper, ICommentService commentService, IActorService actorService, IdentityUser user)
        {
            this.movieService = movieService;
            this.mapper = mapper;
            this.commentService = commentService;
            this.actorService = actorService;
            this.user = user;
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
        public IActionResult Add()
        {
            return this.View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddMovie(AddMovieInputModel model)
        {
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
            if (userId != movie.CreatorId)
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
        public async Task<IActionResult> MyMovies(string id)
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
        public async Task<IActionResult> AddComment(int id, AddCommentInputModel model, string email)
        {
            await movieService.AddComment(id, model, email);

            return RedirectToAction("Details", "Movie", new { id = id });
        }

        [Authorize]
        public IActionResult AddComment(int id)
        {
            return View(id);
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

        public async Task<IActionResult> All()
        {

            var movies = await movieService.GetAllMovie();
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        public async Task<IActionResult> AllTopCommented()
        {

            var movies = await movieService.GetTopCommentedMovie();
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }

        public async Task<IActionResult> AllTopLiked()
        {

            var movies = await movieService.GetTopLikedMovie();
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return this.View(result);
        }


    }
}

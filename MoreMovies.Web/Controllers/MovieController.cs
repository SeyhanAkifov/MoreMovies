using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using MoreMovies.Services.ViewModels;
using MoreMovies.Services.ViewModels.Movie;
using MoreMovies.Web.Models;
using System;
using System.Collections.Generic;
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



        public MovieController(IMovieService movieService, IMapper mapper, ICommentService commentService, IActorService actorService)
        {
            this.movieService = movieService;
            this.mapper = mapper;
            this.commentService = commentService;
            this.actorService = actorService;
        }

        
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

        [HttpGet]
        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie(AddMovieInputModel model)
        {
            await movieService.AddMovie(model);
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await movieService.GetMovieWithId(id);

            var result = mapper.Map<Movie, MovieViewModel>(movie);

            return this.View(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditMovie(int id, AddMovieInputModel model)
        {
            await movieService.EditMovieWithId(id, model);
            
            return RedirectToAction("Details", "Movie", new { id });
        }

        public async Task<IActionResult> MyMovies(string id)
        {
            var movies = await movieService.GetAllMyMovie(id);
            var result = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(movies);

            return View("All", result );
        }


        public async Task<IActionResult> Delete(int id)
        {
           await movieService.DeleteMovie(id);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Like(int id)
        {
            await movieService.LikeMovie(id);

            return RedirectToAction("Details", "Movie", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int id, AddCommentInputModel model, string email)
        {
            await movieService.AddComment(id, model, email);
            
            return RedirectToAction("Details", "Movie", new { id = id });
        }

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


    }
}

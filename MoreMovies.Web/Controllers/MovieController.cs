using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using MoreMovies.Services.ViewModels.Movie;
using MoreMovies.Web.Models;
using System;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class MovieController : Controller
    {

        private readonly IMovieService movieService;
        private readonly ICommentService commentService;
        private readonly IMapper mapper;
        


        public MovieController(IServiceProvider services,IMovieService movieService, ILanguageService languageService, IGenreService genreService, ICountryService countryService, IMapper mapper, ICommentService commentService)
        {
            this.movieService = movieService;
            this.mapper = mapper;
            this.commentService = commentService;
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var movie = await movieService.GetMovieWithId(id);

            var result = mapper.Map<Movie, MovieViewModel>(movie);
            
            result.Comments = this.commentService.GetMovieComments(result.Id);
            return View(result);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult AddMovie(AddMovieInputModel model)
        {
            movieService.AddMovie(model);
            
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
        public IActionResult EditMovie(int id, AddMovieInputModel model)
        {
            movieService.EditMovieWithId(id, model);
            
            return RedirectToAction("Details", "Movie", new { id = id });
        }

        
        public IActionResult Delete(int id)
        {
            movieService.DeleteMovie(id);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Like(int id)
        {
            await movieService.LikeMovie(id);

            return RedirectToAction("Details", "Movie", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int movieId, AddCommentInputModel model)
        {
            await movieService.AddComment(movieId, model);
            
            return RedirectToAction("Details", "Movie", new { id = movieId });
        }

        public IActionResult AddComment(int id)
        {
            return View(id);
        }




    }
}

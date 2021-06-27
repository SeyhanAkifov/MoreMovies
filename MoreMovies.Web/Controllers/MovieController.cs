﻿using AutoMapper;
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
        private readonly IMapper mapper;
        


        public MovieController(IMovieService movieService, IMapper mapper, ICommentService commentService)
        {
            this.movieService = movieService;
            this.mapper = mapper;
            this.commentService = commentService;
        }

        
        public async Task<IActionResult> Details(int id)
        {
            if (id != 0)
            {
                var movie = await movieService.GetMovieWithId(id);

                var result = mapper.Map<Movie, MovieViewModel>(movie);

                result.Comments = this.commentService.GetMovieComments(result.Id);

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
        public async Task<IActionResult> EditMovie(int id, AddMovieInputModel model)
        {
            await movieService.EditMovieWithId(id, model);
            
            return RedirectToAction("Details", "Movie", new { id });
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
        public async Task<IActionResult> AddComment(int movieId, AddCommentInputModel model)
        {
            await movieService.AddComment(movieId, model);
            
            return RedirectToAction("Details", "Movie", new { id = movieId });
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
            
            var topcomentedMovies = await movieService.GetAllMovie();
            var topLikedMovies = await movieService.GetAllMovie();
            var newestAddedMovies = await movieService.GetAllMovie();

            var topCommentedResult = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(topcomentedMovies);
            var topLikedResult = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(topLikedMovies);
            var newestAddedResult = mapper.Map<ICollection<Movie>, ICollection<MovieViewModel>>(newestAddedMovies);

            var movies = new MovieListViewModel()
            {
                TopCommented = topCommentedResult,
                TopLiked = topLikedResult,
                Newest = newestAddedResult
            };


            return this.View(movies);
        }


    }
}

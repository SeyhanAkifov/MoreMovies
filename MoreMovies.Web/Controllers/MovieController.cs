using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using MoreMovies.Services.ViewModels.Movie;
using MoreMovies.Web.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class MovieController : Controller
    {

        private readonly IMovieService movieService;
        private readonly ILanguageService languageService;
        private readonly IGenreService genreService;
        private readonly ICountryService countryService;
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public MovieController( IMovieService movieService, ILanguageService languageService, IGenreService genreService, ICountryService countryService, ApplicationDbContext db, IMapper mapper)
        {
            this.movieService = movieService;
            this.languageService = languageService;
            this.genreService = genreService;
            this.countryService = countryService;
            this.db = db;
            this.mapper = mapper;
        }
        [HttpGet]
        public  IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public  IActionResult AddMovie(AddMovieInputModel model)
        {
            movieService.AddMovie(model);



            return RedirectToAction("Index");
        }
    }
}

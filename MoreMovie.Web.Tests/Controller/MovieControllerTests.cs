using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services;
using MoreMovies.Web.Controllers;
using MoreMovies.Web.Models;
using MoreMovies.Web.Models.Movie;
using MoreMovies.Web.MappingConfiguration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace MoreMovie.Web.Tests.Controller
{
    public class MovieControllerTests
    {
        
        private MovieController PrepareController()
        {
            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService, mapper);

            return new  MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, null);
        }

       

        [Fact]
        public async void AllShoudReturnAllmovies()
        {
            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService, mapper);
            var movieController = new MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, null);

            //Act
            var result = await movieController.All();

            //Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            
            var model = viewResult.Model;
            
            var allViewmodel = Assert.IsType<List<MovieViewModel>>(model);
            Assert.Equal(10, allViewmodel.Count);
        }

        [Fact]
        public void DetailsShouldReturnMovieWithSameId()
        {
            //Arrange

            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService, mapper);
            var movieController = new MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, null);


            //Act

            var result = movieController.Details(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async void RateMovieShouldReturnCorectRatring()
        {
            //Arrange

            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService, mapper);
            var movieController = new MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, null);

            //Act
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            movieController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            await movieController.RateMovie(5, 1);
            var result = await movieController.Details(1);

            //Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = viewResult.Model;

            
            var detailViewModel = Assert.IsType<MovieDetailsViewModel>(model);
            Assert.Equal(5, detailViewModel.Rating);
            Assert.Equal(1, detailViewModel.RatingCount);

        }

        [Fact]
        public async void DeleteSchouldReturnCorrectCount()
        {
            //Arrange

            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService, mapper);
            var movieController = new MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, null);

            //Act
            var moviesActual = data.Movies.Count();
            var movie = data.Movies.First();
            await movieController.Delete(movie.Id);

            //Assert
            Assert.Equal(9, (moviesActual - 1));

        }

        [Fact]
        public async void LikeShouldReturnCorrectLikeCount()
        {
            //Arrange

            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService, mapper);
            var movieController = new MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, null);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            movieController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            //Act
            var movie = data.Movies.First();
            await movieController.Like(movie.Id);


            //Assert
            Assert.Equal(1, movie.Likes);
        }

        [Fact]
        public async void LikeShouldReturnOneLikePerUserCount()
        {
            //Arrange

            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService, mapper);
            var movieController = new MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, null);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            movieController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            //Act
            var movie = data.Movies.First();
            await movieController.Like(movie.Id);
            await movieController.Like(movie.Id);


            //Assert
            Assert.Equal(1, movie.Likes);
        }
    }
}

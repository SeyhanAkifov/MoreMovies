using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Web.Controllers;
using MoreMovies.Web.Hubs;
using MoreMovies.Web.MappingConfiguration;
using MoreMovies.Web.Models;
using MoreMovies.Web.Models.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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

            return new MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, null);
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

        [Fact]
        public async Task AddMovieIfModelStateIsNotValidShouldReturnCorrectRedirect()
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




            IActionResult result = await movieController.AddMovie(new AddMovieInputModel());

            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<AddMovieInputModel>(viewResult.Model);



        }

        [Fact]
        public async Task AddMovieShouldReturnCorrectAddedMovie()
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

            data.Languages.Add(new Language { Name = "English" });
            data.Genre.Add(new Genre { Name = "Action" });
            data.Country.Add(new Country { Name = "USA" });

            data.SaveChanges();

            var movie = new AddMovieInputModel
            {
                Title = "NewTest",
                ReleaseDate = "01/01/2021",
                Description = "dddssssssssssssssssssssssssssssssssssssssssssssssssssssdddddddddd",
                Budget = 200000,
                Creator = "Admin1@abv.bg",
                Image = "http://erere.com",
                HomePage = "http://erere.com",
                Language = "English",
                Country = "USA",
                Genre = "Action",
                Actors = "fgfgf, gggggg",

            };


            IActionResult result = await movieController.AddMovie(movie);

            RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", viewResult.ActionName);



        }

        [Fact]
        public async Task GetDetailsShouldReturnNullWithIdZero()
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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            IActionResult result = await movieController.Details(0);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            Assert.Null(viewResult.Model);


        }

        [Fact]
        public async Task AddShouldReturnCorectView()
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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));

            IActionResult result = await movieController.Add();

            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<AddMovieInputModel>(viewResult.Model);
        }

        [Fact]
        public async Task MyMoviesShouldReturnCorectCount()
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

            IActionResult result = await movieController.MyMovies();

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AddCommentSchoulReturnCorrectView()
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

            IActionResult result = movieController.AddComment();

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AddCommentSchoulReturnCorrectCount()
        {
            var data = DatabaseMock.Instance;

            Mock<IHubClients> mockClients = new Mock<IHubClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            var hubContext = new Mock<IHubContext<MovieHub>>();
            hubContext.Setup(x => x.Clients).Returns(() => mockClients.Object);

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService, mapper);

            var movieController = new MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, hubContext.Object);

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

          

            IActionResult result = await movieController.AddComment(1, new AddCommentInputModel());

            RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Details", viewResult.ActionName);

            var movie = data.Movies.Find(1);

            Assert.Equal(1, movie.Comments.Count);
        }

        [Fact]
        public async Task SearhBygenreShouldReturnCorectCount()
        {
            var data = DatabaseMock.Instance;

            Mock<IHubClients> mockClients = new Mock<IHubClients>();
            Mock<IClientProxy> mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            var hubContext = new Mock<IHubContext<MovieHub>>();
            hubContext.Setup(x => x.Clients).Returns(() => mockClients.Object);

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService, mapper);

            var movieController = new MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, hubContext.Object);

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

            data.Genre.Add(new Genre { Name = "Action" });
            data.MovieGenres.Add(new MovieGenre { MovieId = 1, GenreId = 1 });

            data.SaveChanges();

            IActionResult result = await movieController.SearchByGenre("Action");

            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<List<MovieViewModel>>(viewResult.Model);

            var expectedCount = 1;

            Assert.Equal(expectedCount, model.Count);
        }

        [Fact]
        public void MigrationsUpDownTest()
        {
            // Unit tests don't have a DataDirectory by default to store DB in
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());

            // Drop and recreate database
            ApplicationDbContext db = new ApplicationDbContext();
            db.Database.EnsureDeleted();

            // Retrieve migrations
            db.Database.Migrate();

            // Optional: delete database
            db.Database.EnsureDeleted();
        }

    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Models;
using MoreMovies.Services;
using MoreMovies.Web.Controllers;
using MoreMovies.Web.Models;
using SocialNetworkCustom.Web.MappingConfiguration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests.Controller
{
    public class MovieControllerTests
    {
        [Fact]
        public async void AllShoudReturnAllmovies()
        {
            var data = DatabaseMock.Instance;
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));

            data.Movies.AddRange(Enumerable.Range(0, 10).Select(i => new Movie()));
            data.SaveChanges();
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

            var except = await movieService.GetAllMovie();

            var model = viewResult.Model;

            var allViewmodel = Assert.IsType<List<MovieViewModel>>(model);
            Assert.Equal(except.Count, allViewmodel.Count);



        }
    }
}

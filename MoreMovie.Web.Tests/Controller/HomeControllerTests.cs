using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Models;
using MoreMovies.Services;
using MoreMovies.Web.Controllers;
using MoreMovies.Web.Models;
using SocialNetworkCustom.Web.MappingConfiguration;
using System.Linq;
using Xunit;

namespace MoreMovie.Web.Tests.Controller
{
    public class HomeControllerTests
    {
        [Fact]
        public async void IndexShouldReturnViewWithCorrectModel()
        {
            //Arrange
            var data = DatabaseMock.Instance;
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));

            data.Movies.AddRange(Enumerable.Range(0, 10).Select(i => new Movie()));
            data.SaveChanges();
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var comingSoonService = new ComingSoonService(data);

            var movieService = new MovieService(null, commentService, data, languageService, genreService, countryService);

            var homeController = new HomeController(null, movieService, mapper, data, newsService, comingSoonService, genreService);

            //Act
            var result = await homeController.Index();

            //Assert
            Assert.NotNull(result);
            var viewResult  = Assert.IsType<ViewResult>(result);

            var model = viewResult.Model;

            var topLiked = await movieService.GetTopLikedMovie();
            var topCommented = await movieService.GetTopCommentedMovie();
            var newest = await movieService.GetNewestAddedMovie();

            var indexViewModel = Assert.IsType<MovieListViewModel>(model);

            Assert.Equal(topLiked.Count, indexViewModel.TopLiked.Count);
            Assert.Equal(topCommented.Count, indexViewModel.TopCommented.Count);
            Assert.Equal(newest.Count, indexViewModel.Newest.Count);

        }

        [Fact]
        public void ErrorShouldReturnView()
        {
            //Arrange
            var homeController = new HomeController(null, null, MapperMock.Instanse, null, null, null, null);

            //Act
            var result = homeController.Error();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

        }
    }
}

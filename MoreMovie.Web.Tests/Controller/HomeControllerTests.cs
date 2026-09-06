using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Services;
using MoreMovies.Web.Controllers;
using MoreMovies.Web.MappingConfiguration;
using MoreMovies.Web.Models;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests.Controller
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task IndexShouldReturnViewWithCorrectModel()
        {
            //Arrange
            var data = DatabaseMock.Instance;
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var cache = new MemoryCache(new MemoryCacheOptions());

            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var comingSoonService = new ComingSoonService(data);

            var movieService = new MovieService(commentService, data, languageService, genreService, countryService);

            var homeController = new HomeController(mapper, newsService, comingSoonService, genreService, cache, null, null, movieService);

            //Act
            var result = await homeController.Index();

            //Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = viewResult.Model;

            var topLiked = await movieService.GetTopLikedMovie();
            var topCommented = await movieService.GetTopCommentedMovie();
            var newest = await movieService.GetNewestAddedMovie();
            var news = await newsService.GetNewsForHomePage();
            var comingSoon = await comingSoonService.GetForHomePage();

            var indexViewModel = Assert.IsType<MovieListViewModel>(model);

            Assert.Equal(topLiked.Count, indexViewModel.TopLiked.Count);
            Assert.Equal(topCommented.Count, indexViewModel.TopCommented.Count);
            Assert.Equal(newest.Count, indexViewModel.Newest.Count);
            Assert.Equal(news.Count, indexViewModel.News.Count);
            Assert.Equal(comingSoon.Count, indexViewModel.ComingSoon.Count);
        }

        [Fact]
        public void ErrorShouldReturnView()
        {
            //Arrange
            var homeController = new HomeController(MapperMock.Instance, null, null, null, null, null, null, null);

            //Act
            var result = homeController.Error();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }
    }
}

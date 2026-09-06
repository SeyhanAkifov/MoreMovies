using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Models;
using MoreMovies.Services;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Web.Controllers;
using MoreMovies.Web.MappingConfiguration;
using MoreMovies.Web.Models.News;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests.Controller
{
    public class NewsControllerTests
    {
        [Fact]
        public async Task Test()
        {
            var data = DatabaseMock.Instance;
            var newsService = new NewsService(data);
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));

            var newsController = new NewsController(newsService, mapper);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, RoleNames.User),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            newsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            await newsController.Add(new NewsAddModel { Title = "Test news title", Description = "Test news description, long enough" });

            var allNews = data.News.ToList();

            Assert.Equal(allNews.Count, 1);

        }

        [Fact]
        public async Task DeleteShouldReturnCorrectCount()
        {
            var data = DatabaseMock.Instance;
            var newsService = new NewsService(data);
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));

            var newsController = new NewsController(newsService, mapper);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, RoleNames.User),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            newsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            await newsController.Add(new NewsAddModel { Title = "Test news title", Description = "Test news description, long enough" });
            await newsController.Add(new NewsAddModel { Title = "Test news title", Description = "Test news description, long enough" });
            await newsController.Delete(1);

            var allNews = data.News.ToList();

            Assert.Equal(allNews.Count, 1);

        }

        [Fact]
        public async Task AllShouldReturnCorrectCount()
        {
            var data = DatabaseMock.Instance;
            var newsService = new NewsService(data);
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));

            var newsController = new NewsController(newsService, mapper);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, RoleNames.User),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            newsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            await newsController.Add(new NewsAddModel { Title = "Test news title", Description = "Test news description, long enough" });
            await newsController.Add(new NewsAddModel { Title = "Test news title", Description = "Test news description, long enough" });
            await newsController.All();

            var allNews = data.News.ToList();

            Assert.Equal(allNews.Count, 2);

        }

        [Fact]
        public async Task DetailsShouldReturnCorrect()
        {
            var data = DatabaseMock.Instance;
            var newsService = new NewsService(data);
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));

            var newsController = new NewsController(newsService, mapper);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, RoleNames.User),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            newsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            await newsController.Add(new NewsAddModel { Title = "Test news title", Description = "Test news description, long enough" });





            IActionResult result = await newsController.Details(1);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<NewsViewModel>(viewResult.Model);



        }
    }
}

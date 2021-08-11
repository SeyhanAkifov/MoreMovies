using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Models;
using MoreMovies.Services;
using MoreMovies.Services.Interfaces;
using System;
using System.Linq;
using Xunit;

namespace MoreMovie.Web.Tests.Service
{
    public class NewsServiceTests
    {
        [Fact]
        public static async void Test()
        {
            //Arrange
            var newsService = GetNewsService();

            //Act
            var result = await newsService.GetNewsWithId(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
            Assert.Equal("Test", result.Description);
            
        }

        private static INewsService GetNewsService()
        {
            var data = DatabaseMock.Instance;

            data.News.Add(new News
            {
                Id = 1,
                Title = "Test", 
                Description = "Test",
                AddedOn = new DateTime(01 / 01 / 2020)
            });

            data.SaveChanges();

            var newsService = new NewsService(data);

            return newsService;
        }

        [Fact]
        public void AllNewsShouldReturnCorrectCount()
        {
            var data = DatabaseMock.Instance;

            data.News.Add(new News
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                AddedOn = new DateTime(01 / 01 / 2020)
            });

            data.SaveChanges();

            var newsService = new NewsService(data);

            var result = newsService.GetAllNews();


            Assert.Equal(1, result.Result.Count);
        }

        [Fact]
        public void GetNewsWithIdShouldReturnCorrectNews()
        {
            var data = DatabaseMock.Instance;

            data.News.Add(new News
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                AddedOn = new DateTime(01 / 01 / 2020)
            });

            data.SaveChanges();

            var newsService = new NewsService(data);

            var result = newsService.GetNewsWithId(1);

            Assert.Equal("Test", result.Result.Title);
            Assert.Equal("Test", result.Result.Description);
            Assert.Equal(1, result.Result.Id);
            
        }

        [Fact]
        public void GetAllNewsShouldReturnCorrectCount()
        {
            var data = DatabaseMock.Instance;

            data.News.AddRange(Enumerable.Range(0, 10).Select(i => new News()));

            data.SaveChanges();

            var newsService = new NewsService(data);

            var result = newsService.GetAllNews();

            Assert.Equal(10, result.Result.Count);
        }
    }
}

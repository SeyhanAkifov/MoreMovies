using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Models;
using MoreMovies.Services;
using MoreMovies.Services.Interfaces;
using System;
using Xunit;

namespace MoreMovie.Web.Tests.Service
{
    public class NewsServiceTests
    {
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
    }
}

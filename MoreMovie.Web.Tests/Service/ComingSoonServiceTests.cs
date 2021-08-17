using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Models;
using MoreMovies.Services;
using System;
using Xunit;

namespace MoreMovie.Web.Tests.Service
{
    public class ComingSoonServiceTests
    {
        [Fact]
        public static async void Test()
        {
            //Arrange
            var data = DatabaseMock.Instance;

            data.ComingSoons.Add(new ComingSoon
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                AddedOn = new DateTime(01 / 01 / 2020)
            });

            data.SaveChanges();

            var comingSoonService = new ComingSoonService(data);

            //Act
            var result = await comingSoonService.GetWithId(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
            Assert.Equal("Test", result.Description);

        }
    }
}

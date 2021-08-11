using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests
{
    public class BasicTests : IClassFixture<WebApplicationFactory<MoreMovies.Web.Startup>>
    {
        private readonly WebApplicationFactory<MoreMovies.Web.Startup> _factory;

        public BasicTests(WebApplicationFactory<MoreMovies.Web.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Index")]
        [InlineData("/Contact/ContactPage")]
        [InlineData("/Movie/Details/0")]
        [InlineData("/Movie/MyMovies")]
        [InlineData("/Movie/Add")]
        [InlineData("/Movie/AddComment")]
        [InlineData("/Movie/All")]
        [InlineData("/Movie/AllTopCommented")]
        [InlineData("/Movie/AllTopLiked")]
        [InlineData("/ComingSoon/All")]
        [InlineData("/News/All")]

        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            
        }

        [Fact]
        public async Task HomePage()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            var html  = await response.Content.ReadAsStringAsync();

            Assert.Contains(" <h2>LATEST TRAILERS</h2>", html);

            
        }
    }
}

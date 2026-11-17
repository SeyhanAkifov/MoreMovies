using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MoreMovies.Data;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovies.Web.Tests
{
    public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public BasicTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("MoreMoviesEndpointTests"));
                }));
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Index")]
        [InlineData("/Contact/ContactPage")]
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
        public async Task MovieDetailsRequiresAuthentication()
        {
            using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            using var response = await client.GetAsync("/Movie/Details?id=1");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Theory]
        [InlineData("/Movie/Like")]
        [InlineData("/Movie/DisLike")]
        [InlineData("/Movie/RateMovie")]
        [InlineData("/Home/Logout")]
        [InlineData("/News/Delete")]
        public async Task WriteActionsRejectGet(string url)
        {
            using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            using var response = await client.GetAsync(url);

            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
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

            Assert.Contains("Latest trailers", html, StringComparison.OrdinalIgnoreCase);
        }
    }
}

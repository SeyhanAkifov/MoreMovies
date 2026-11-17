using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MoreMovies.Data;
using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
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

        [Fact]
        public async Task SignInReturnsToRequestedMovie()
        {
            using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            using var scope = _factory.Services.CreateScope();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            const string email = "login-test@example.com";
            const string password = "Test-Only!957";
            Assert.True((await users.CreateAsync(new IdentityUser
            {
                Email = email,
                UserName = email,
                EmailConfirmed = true
            }, password)).Succeeded);

            const string movie = "/Movie/Details?id=1";
            using var challenge = await client.GetAsync(movie);
            var loginUrl = challenge.Headers.Location;
            Assert.Equal("/Identity/Account/Login", loginUrl.AbsolutePath);

            var loginPage = await client.GetStringAsync(loginUrl);
            Assert.Contains("login-screen", loginPage);
            var token = Regex.Match(loginPage, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value;
            Assert.NotEmpty(token);

            var form = new Dictionary<string, string>
            {
                ["Input.Email"] = email,
                ["Input.Password"] = password,
                ["ReturnUrl"] = movie
            };

            using var missingToken = await client.PostAsync(loginUrl, new FormUrlEncodedContent(form));
            Assert.Equal(HttpStatusCode.BadRequest, missingToken.StatusCode);

            form["__RequestVerificationToken"] = token;
            form["Input.Password"] = "Incorrect-password!8";
            using var rejected = await client.PostAsync(loginUrl, new FormUrlEncodedContent(form));
            Assert.Equal(HttpStatusCode.OK, rejected.StatusCode);
            Assert.Contains("Sign-in failed.", await rejected.Content.ReadAsStringAsync());

            form["Input.Password"] = password;
            using var signedIn = await client.PostAsync(loginUrl, new FormUrlEncodedContent(form));
            Assert.Equal(HttpStatusCode.Redirect, signedIn.StatusCode);
            Assert.Equal(movie, signedIn.Headers.Location.OriginalString);

            using var details = await client.GetAsync(movie);
            Assert.Equal(HttpStatusCode.OK, details.StatusCode);
            var moviePage = await details.Content.ReadAsStringAsync();
            Assert.Contains("main navigation", moviePage, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("camera=(), microphone=()", details.Headers.GetValues("Permissions-Policy"));
            var logoutToken = Regex.Match(moviePage, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value;
            Assert.NotEmpty(logoutToken);

            var ratingForm = new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = logoutToken,
                ["movieId"] = "1",
                ["rating"] = "0"
            };
            using var invalidRating = await client.PostAsync("/Movie/RateMovie", new FormUrlEncodedContent(ratingForm));
            Assert.Equal(HttpStatusCode.BadRequest, invalidRating.StatusCode);

            ratingForm["rating"] = "4";
            using var validRating = await client.PostAsync("/Movie/RateMovie", new FormUrlEncodedContent(ratingForm));
            Assert.Equal(HttpStatusCode.Redirect, validRating.StatusCode);
            var updatedMovie = await client.GetStringAsync(movie);
            Assert.Contains("4 / 5, 1 vote", updatedMovie);

            using var logout = await client.PostAsync("/Home/Logout", new FormUrlEncodedContent(
                new Dictionary<string, string> { ["__RequestVerificationToken"] = logoutToken }));
            Assert.Equal(HttpStatusCode.Redirect, logout.StatusCode);

            using var signedOut = await client.GetAsync(movie);
            Assert.Equal(HttpStatusCode.Redirect, signedOut.StatusCode);
            Assert.Equal("/Identity/Account/Login", signedOut.Headers.Location.AbsolutePath);
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

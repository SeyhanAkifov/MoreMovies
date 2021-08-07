using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Controllers.Api;
using MoreMovies.Web.Models.Movie;
using Xunit;

namespace MoreMovie.Web.Tests.Controller.Api
{
    public class MovieApiControllerTests
    {
        private readonly Mock<IMapper> mapper;
        private readonly Mock<IMovieService> service;
        private readonly DetailsApiController controller;

        public MovieApiControllerTests()
        {
            this.mapper = new Mock<IMapper>();
            this.service = new Mock<IMovieService>();
            this.controller = new DetailsApiController(this.service.Object, this.mapper.Object);
        }

       

        [Fact]
        public async void ReturnCorrectRequest()
        {

            IActionResult result = await controller.GetDetails(1);

            Assert.IsType<OkObjectResult>(result);

           
        }

        [Fact]
        public async void ReturnBadRequest()
        {

            IActionResult result = await controller.GetDetails(0);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("Invalid request for id 0", badRequestResult.Value);
        }
    }
}

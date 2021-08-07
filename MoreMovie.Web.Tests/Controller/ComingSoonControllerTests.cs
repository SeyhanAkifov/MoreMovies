using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Controllers;
using MoreMovies.Web.Models;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests.Controller
{
    public class ComingSoonControllerTests
    {
        private readonly Mock<IMapper> mapper;
        private readonly Mock<IComingSoonService> service;
        private readonly ComingSoonController controller;

        public ComingSoonControllerTests()
        {
            this.mapper = new Mock<IMapper>();
            this.service = new Mock<IComingSoonService>();
            this.controller = new ComingSoonController(this.service.Object, this.mapper.Object);
        }

        [Fact]
        public async void ReturnViewAll()
        {
            IActionResult result = await controller.All();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ReturnViewAdd()
        {
            IActionResult result = controller.Add();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void ReturnViewInvalidmodelState()
        {
            controller.ModelState.AddModelError("x", "Test Error");

            var comingSoon = new ComingSoonAddModel
            {
                Title = "",
                Description = "",
                ImageUrl = ""
            };

            IActionResult result = await controller.Add(comingSoon);

            ViewResult viewResult  = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ComingSoonAddModel>(viewResult.Model);

            Assert.Equal(comingSoon.Title, model.Title);
        }

        [Fact]
        public async Task NotSaveComingSoonWhenmodelError()
        {
            controller.ModelState.AddModelError("x", "Test Error");

            var comingSoon = new ComingSoonAddModel
            {
                Title = "",
                Description = "",
                ImageUrl = ""
            };

            IActionResult result = await controller.Add(comingSoon);

            service.Verify(x => x.Add(It.IsAny<ComingSoonAddModel>()), Times.Never);
        }

        [Fact]
        public async Task SaveComingSoonWhenValidModel()
        {
            ComingSoonAddModel savedComingSoon = null;

            

            service.Setup(x => x.Add(It.IsAny<ComingSoonAddModel>()))
                .Returns(Task.CompletedTask)
                .Callback<ComingSoonAddModel>(x => savedComingSoon = x);

            var comingSoon = new ComingSoonAddModel
            {
                Title = "",
                Description = "",
                ImageUrl = ""
            };

            await controller.Add(comingSoon);

            service.Verify(x => x.Add(It.IsAny<ComingSoonAddModel>()), Times.Once);

            Assert.Equal(comingSoon.Description, savedComingSoon.Description);
        }

        [Fact]
        public async Task SaveComingSoonDetailsWhenValidModel()
        {
            var comingSoon = new ComingSoonAddModel
            {
                Title = "New Soon",
                Description = "Coming",
                ImageUrl = "url"
            };

            await controller.Add(comingSoon);

            IActionResult result = await controller.Details(3);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<ComingSoonViewModel>(viewResult.Model);

            Assert.Equal(comingSoon.Title, model.Title);
            Assert.Equal(comingSoon.Title, model.Title);
            Assert.Equal(comingSoon.Title, model.Title);
        }

        [Fact]
        public async void ReturnBadRequest()
        {

            IActionResult result = await controller.Details(0);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("Invalid request for id 0", badRequestResult.Value);
        }

        [Fact]
        public async Task ReturnCompleteViewWhenValidmodel()
        {
            var comingSoon = new ComingSoonAddModel
            {
                Title = null,
                Description = "",
                ImageUrl = ""
            };

            IActionResult result = await controller.Add(comingSoon);

            RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", viewResult.ActionName);
            Assert.Equal("Home", viewResult.ControllerName);

        }
    }
}

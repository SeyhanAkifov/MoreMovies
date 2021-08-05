using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Services;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Web.Controllers;
using SocialNetworkCustom.Web.MappingConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests.Controller
{
    public class ComingSoonTests
    {
        [Fact]
        public async void Test()
        {
            var data = DatabaseMock.Instance;
            var comingSoonService = new ComingSoonService(data);
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));

            var comingSoonController = new ComingSoonController(comingSoonService, mapper);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "User"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            comingSoonController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            await comingSoonController.Add(new ComingSoonAddModel());

            var allComingSoon = data.ComingSoons.ToList();

            Assert.Equal(allComingSoon.Count, 1);

        }

        [Fact]
        public async void DeleteShouldReturnCorrectCount()
        {
            var data = DatabaseMock.Instance;
            var comingSoonService = new ComingSoonService(data);
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));

            var comingSoonController = new ComingSoonController(comingSoonService, mapper);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "User"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            comingSoonController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            await comingSoonController.Add(new ComingSoonAddModel());
            await comingSoonController.Add(new ComingSoonAddModel());
            await comingSoonController.Delete(1);

            var allComingSoon = data.ComingSoons.ToList();

            Assert.Equal(allComingSoon.Count, 1);

        }

        [Fact]
        public async void AllShouldReturnCorrectCount()
        {
            var data = DatabaseMock.Instance;
            var comingSoonService = new ComingSoonService(data);
            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));

            var comingSoonController = new ComingSoonController(comingSoonService, mapper);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "example name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "User"),
                new Claim("custom-claim", "example claim value"),
            }, "mock"));


            comingSoonController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            await comingSoonController.Add(new ComingSoonAddModel());
            await comingSoonController.Add(new ComingSoonAddModel());
            await comingSoonController.All();

            var allComingSoon = data.ComingSoons.ToList();

            Assert.Equal(allComingSoon.Count, 2);

        }
    }
}

using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using System;
using System.Linq;

namespace MoreMovie.Web.Tests.Mocks
{
    public static class DatabaseMock
    {
        public static ApplicationDbContext Instance
        {
            get
            {
                var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

                var data  = new ApplicationDbContext(dbContextOptions);

                data.Movies.AddRange(Enumerable.Range(0, 10).Select(i => new Movie
                {
                    Creator = "Admin1@abv.bg",
                    Title = $"Test movie {i}",
                    Description = "Test movie description",
                    ReleaseDate = DateTime.UtcNow.AddDays(-i),
                    Budget = 1000,
                    HomePage = "http://test.com",
                    ImageUrl = "http://test.com/image.jpg",
                }));
                data.SaveChanges();

                return data;
            }
        }
    }
}

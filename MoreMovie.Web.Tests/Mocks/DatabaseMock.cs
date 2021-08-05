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

                data.Movies.AddRange(Enumerable.Range(0, 10).Select(i => new Movie()));
                data.SaveChanges();

                return data;
            }
        }
    }
}

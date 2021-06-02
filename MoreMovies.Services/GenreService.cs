using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class GenreService : IGenreService
    {
        private readonly ApplicationDbContext dbContext;
        public GenreService()
        {
            this.dbContext = new ApplicationDbContext();
        }
        public void Add(string name)
        {
            Genre genre = new Genre()
            {
                Name = name
            };

            this.dbContext.Genre.Add(genre);

            this.dbContext.SaveChanges();

        }
    }
}

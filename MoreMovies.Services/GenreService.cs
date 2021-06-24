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
        private readonly ApplicationDbContext db;
        public GenreService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public void Add(string name)
        {
            Genre genre = new Genre()
            {
                Name = name
            };

            this.db.Genre.Add(genre);

            this.db.SaveChanges();

        }
    }
}

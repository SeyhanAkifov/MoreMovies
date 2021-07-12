using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using System.Collections.Generic;
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

        public async Task Add(string name)
        {
            Genre genre = new()
            {
                Name = name
            };

            this.db.Genre.Add(genre);

            await this.db.SaveChangesAsync();
        }

        public async Task<ICollection<Genre>> GetGenres()
        {
            return await this.db.Genre.ToListAsync();
        }
    }
}

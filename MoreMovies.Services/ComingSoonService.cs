using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto;
using MoreMovies.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class ComingSoonService : IComingSoonService
    {
        private readonly ApplicationDbContext db;

        public ComingSoonService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task Add(ComingSoonAddModel model)
        {
            var item = new ComingSoon()
            {
                Title = model.Title,
                Description = model.Description,
                ImageUrl = model.ImageUrl
            };

            this.db.ComingSoons.Add(item);

            await this.db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var item = await this.db.ComingSoons.FindAsync(id);

            this.db.ComingSoons.Remove(item);

            await this.db.SaveChangesAsync();
        }

        public async Task<ICollection<ComingSoon>> GetAll()
        {
            var item = await this.db.ComingSoons.Select(x => x).ToArrayAsync();

            return item;
        }

        public async Task<ICollection<ComingSoon>> GetForHomePage()
        {
            var item = await this.db.ComingSoons.Select(x => x).OrderByDescending(x => x.AddedOn).Take(2).ToArrayAsync();

            return item;
        }

        public async Task<ComingSoon> GetWithId(int id)
        {
            var news = await this.db.ComingSoons.FindAsync(id);

            return news;
        }
    }
}

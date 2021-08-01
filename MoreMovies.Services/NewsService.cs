using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext db;

        public NewsService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task Add(NewsAddModel model)
        {
            var news = new News()
            {
                Title = model.Title,
                Description = model.Description
            };

            this.db.News.Add(news);

            await this.db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var news = await this.db.News.FindAsync(id);

            this.db.News.Remove(news);

            await this.db.SaveChangesAsync();
        }

        public async Task<ICollection<News>> GetAllNews()
        {
            var news = await this.db.News.Select(x => x).ToArrayAsync();

            return news;
        }

        public async Task<ICollection<News>> GetNewsForHomePage()
        {
            var news = await this.db.News.Select(x => x).OrderByDescending(x => x.AddedOn).Take(3).ToArrayAsync();

            return news;
        }

        public async Task<News> GetNewsWithId(int id)
        {
            var news = await this.db.News.FindAsync(id);

            return news;
        }
    }
}

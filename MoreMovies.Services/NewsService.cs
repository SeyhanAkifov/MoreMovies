using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
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

        public async Task<ICollection<NewsOutputDto>> GetAllNews()
        {
            var news = await this.db.News
                .Select(x => new NewsOutputDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description
                })
                .ToArrayAsync();

            return news;
        }

        public async Task<ICollection<NewsOutputDto>> GetNewsForHomePage()
        {
            var news = await this.db.News
                .OrderByDescending(x => x.AddedOn)
                .Take(3)
                .Select(x => new NewsOutputDto 
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description
                })
                .ToArrayAsync();

            return news;
        }

        public async Task<NewsOutputDto> GetNewsWithId(int id)
        {
            var news = await this.db.News.FindAsync(id);
            
            var result  = new NewsOutputDto
            {
                Id = news.Id,
                Title = news.Title,
                Description = news.Description
            };

            return result;
        }
    }
}

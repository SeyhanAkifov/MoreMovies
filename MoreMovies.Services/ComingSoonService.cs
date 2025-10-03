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
    public class ComingSoonService : IComingSoonService
    {
        private readonly ApplicationDbContext db;

        public ComingSoonService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task Add(ComingSoonAddModel model)
        {
            var comingSoon = new ComingSoon()
            {
                Title = model.Title,
                Description = model.Description,
                ImageUrl = model.ImageUrl
            };

            this.db.ComingSoons.Add(comingSoon);

            await this.db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var comingSoon = await this.db.ComingSoons.FindAsync(id);

            this.db.ComingSoons.Remove(comingSoon);

            await this.db.SaveChangesAsync();
        }

        public async Task<ICollection<ComingSoonOutputDto>> GetAll()
        {
            var comingSoon = await this.db.ComingSoons
                .Select(x => GetComingSoonOutputDto(x))
                .ToArrayAsync();

            return comingSoon;
        }

        public async Task<ICollection<ComingSoonOutputDto>> GetForHomePage()
        {
            var comingSoon = await this.db.ComingSoons
                .OrderByDescending(x => x.AddedOn)
                .Take(3)
                 .Select(x => GetComingSoonOutputDto(x))
                 .ToArrayAsync();

            return comingSoon;
        }

        public async Task<ComingSoonOutputDto> GetWithId(int id)
        {
            var comingSoon = await this.db.ComingSoons.FindAsync(id);

            var result = GetComingSoonOutputDto(comingSoon);

            return result;
        }

        private static ComingSoonOutputDto GetComingSoonOutputDto(ComingSoon comingSoon)
        {
            return new ComingSoonOutputDto
            {
                Id = comingSoon.Id,
                Title = comingSoon.Title,
                Description = comingSoon.Description,
                ImageUrl = comingSoon.ImageUrl
            };
        }
    }
}

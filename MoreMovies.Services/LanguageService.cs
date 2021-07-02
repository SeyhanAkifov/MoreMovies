using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ApplicationDbContext db;
        public LanguageService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task Add(string name)
        {
            Language language = new()
            {
                Name = name
            };

            this.db.Languages.Add(language);
            await this.db.SaveChangesAsync();
        }
    }
}

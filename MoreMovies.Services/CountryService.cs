using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class CountryService : ICountryService
    {
        private readonly ApplicationDbContext db;
        public CountryService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task Add(string name)
        {
            Country country = new()
            {
                Name = name
            };

            this.db.Country.Add(country);
            await this.db.SaveChangesAsync();
        }

        public async Task<ICollection<Country>> GetCountries()
        {
            return await this.db.Country.ToListAsync();
        }
    }
}

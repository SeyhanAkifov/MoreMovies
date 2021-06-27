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
    public class CountryService : ICountryService
    {
        private readonly ApplicationDbContext db;
        public CountryService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void Add(string name)
        {
            Country country = new()
            {
                Name = name
            };

            this.db.Country.Add(country);
            this.db.SaveChanges();

        }
    }
}

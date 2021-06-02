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
        private readonly ApplicationDbContext dbContext;
        public CountryService()
        {
            this.dbContext = new ApplicationDbContext();
        }

        public void Add(string name)
        {
            Country language = new Country()
            {
                Name = name
            };

            this.dbContext.Country.Add(language);
            this.dbContext.SaveChanges();

        }
    }
}

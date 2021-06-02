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
    public class LanguageService : ILanguageService
    {
        private readonly ApplicationDbContext dbContext;
        public LanguageService()
        {
            this.dbContext = new ApplicationDbContext();
        }

        public  void Add(string name)
        {
            Language language = new Language()
            {
                Name = name
            };

            this.dbContext.Languages.Add(language);
            this.dbContext.SaveChanges();
           
        }
    }
}

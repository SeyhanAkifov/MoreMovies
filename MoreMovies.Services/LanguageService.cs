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
        private readonly ApplicationDbContext db;
        public LanguageService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public  void Add(string name)
        {
            Language language = new()
            {
                Name = name
            };

            this.db.Languages.Add(language);
            this.db.SaveChanges();
        }
    }
}

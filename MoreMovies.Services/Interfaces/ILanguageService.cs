using MoreMovies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface ILanguageService
    {
        public Task Add(string name);

        public Task<ICollection<Language>> GetLanguages();
    }
}

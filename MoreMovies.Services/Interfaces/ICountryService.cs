using MoreMovies.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface ICountryService
    {
        public Task Add(string name);

        public Task<ICollection<Country>> GetCountries();
    }
}

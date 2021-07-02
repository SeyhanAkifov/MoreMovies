using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface ICountryService
    {
        public Task Add(string name);
    }
}

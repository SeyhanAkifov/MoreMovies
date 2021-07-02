using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface IGenreService
    {
        public Task Add(string name);
    }
}

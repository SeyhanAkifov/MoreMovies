using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface IActorService
    {
        public Task Add();

        public List<string> GetMovieActors(int id);
    }
}

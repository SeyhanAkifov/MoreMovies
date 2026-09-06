using System.Collections.Generic;

namespace MoreMovies.Services.Interfaces
{
    public interface IActorService
    {
        public List<string> GetMovieActors(int id);
    }
}

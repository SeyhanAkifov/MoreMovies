using MoreMovies.Data;
using MoreMovies.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MoreMovies.Services
{
    public class ActorService : IActorService
    {
        private readonly ApplicationDbContext db;

        public ActorService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public List<string> GetMovieActors(int id)
        {
            var d = this.db.MovieActors
                .Where(x => x.MovieId == id)
                .Join(this.db.Actors,
                a => a.ActorId,
                b => b.Id,
                (a, b) => b.Name).ToList();

            return d;
        }
    }
}

using MoreMovies.Models;
using MoreMovies.Services.ViewModels.Movie;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface IMovieService
    {
        public void AddMovie(AddMovieInputModel movie);

        public Task DeleteMovie(int id);

        public Task<int> SearchMovie(string name);

        public Task LikeMovie(int id);

        public Task AddComment(int id, AddCommentInputModel model);

        public Task EditMovieWithId(int id, AddMovieInputModel model);

        public Task<Movie> GetMovieWithId(int id);

        public Task<ICollection<Movie>> GetAllMovie();
    }
}

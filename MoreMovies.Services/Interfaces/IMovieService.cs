using MoreMovies.Models;
using MoreMovies.Services.ViewModels.Movie;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface IMovieService
    {
        public void AddMovie(AddMovieInputModel movie);

        public void DeleteMovie(int id);

        public void SearchMovie();

        public Task LikeMovie(int id);

        public Task AddComment(int id, AddCommentInputModel model);

        public void EditMovieWithId(int id, AddMovieInputModel model);

        public Task<Movie> GetMovieWithId(int id);

        public ICollection<Movie> GetAllMovie();
    }
}

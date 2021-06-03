using MoreMovies.Models;
using MoreMovies.Services.ViewModels.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface IMovieService
    {
        public  void AddMovie(AddMovieInputModel movie);
        public  void DeleteMovie(int id);
        public  void SearchMovie();
        public void EditMovieWithId(int id, AddMovieInputModel model);
        public  Movie GetMovieWithId(int id);
        public  ICollection<Movie> GetAllMovie();

    }
}

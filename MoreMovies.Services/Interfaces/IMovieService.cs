﻿using MoreMovies.Models;
using MoreMovies.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface IMovieService
    {
        public Task AddMovie(AddMovieInputModel movie);

        public Task DeleteMovie(int id);

        public Task<int> SearchMovie(string name);

        public Task LikeMovie(int id);

        public Task AddComment(int id, AddCommentInputModel model, string email);

        public Task EditMovieWithId(int id, AddMovieInputModel model);

        public Task<Movie> GetMovieWithId(int id);

        public Task<ICollection<Movie>> GetAllMovie();

        public Task<ICollection<Movie>> GetAllMyMovie(string email);

        public Task<ICollection<Movie>> GetTopCommentedMovie();

        public Task<ICollection<Movie>> GetTopLikedMovie();

        public Task<ICollection<Movie>> GetNewestAddedMovie();
    }
}

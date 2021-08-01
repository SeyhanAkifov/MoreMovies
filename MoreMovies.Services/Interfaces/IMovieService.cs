﻿using MoreMovies.Models;
using MoreMovies.Services.Dto;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface IMovieService
    {
        public Task AddMovie(AddMovieInputModel movie);

        public Task DeleteMovie(int id);

        public Task<int> SearchMovie(string name);

        public Task<ICollection<Movie>> SearchMovieByGenre(string genre);

        public Task<ICollection<Movie>> SearchMovieByYear(string year);

        public Task LikeMovie(int id);

        public Task AddComment(AddCommentInputModel model);

        public Task EditMovieWithId(int id, EditMovieInputModel model);

        public Task<MovieOutputDto> GetMovieWithId(int id);

        public Task<ICollection<Movie>> GetAllMovie();

        public Task<ICollection<Movie>> GetAllMyMovie(string email);

        public Task<ICollection<Movie>> GetTopCommentedMovie();

        public Task<ICollection<Movie>> GetTopCommentedAllMovie();

        public Task<ICollection<Movie>> GetTopLikedMovie();

        public Task<ICollection<Movie>> GetTopLikedAllMovie();

        public Task<ICollection<Movie>> GetNewestAddedMovie();

        public Task<ICollection<Movie>> GetNewestAddedAllMovie();

        public Task Ratemovie(int rating, int movieId);

        public Task<DetailsOutputDto> GetDetails(int id);
    }
}

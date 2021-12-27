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

        public Task<ICollection<MovieOutputDto>> SearchMovie(string name);

        public Task<ICollection<MovieOutputDto>> SearchMovieByGenre(string genre);

        public Task<ICollection<MovieOutputDto>> SearchMovieByYear(string year);

        public Task LikeMovie(int id, string userId);

        public Task DisLikeMovie(int id, string userId);

        public Task AddComment(AddCommentInputModel model);

        public Task EditMovieWithId(int id, EditMovieInputModel model);

        public Task<MovieDetailOutputDto> GetMovieWithId(int id);

        public Task<ICollection<MovieOutputDto>> GetAllMovie();

        public Task<ICollection<MovieOutputDto>> GetAllMyMovie(string email);

        public Task<ICollection<MovieOutputDto>> GetAllMyLiked(string userName);

        public Task<ICollection<MovieOutputDto>> GetTopCommentedMovie();

        public Task<ICollection<MovieOutputDto>> GetTopCommentedAllMovie();

        public Task<ICollection<MovieOutputDto>> GetTopLikedMovie();

        public Task<ICollection<MovieOutputDto>> GetTopLikedAllMovie();

        public Task<ICollection<MovieOutputDto>> GetNewestAddedMovie();

        public Task<ICollection<MovieOutputDto>> GetNewestAddedAllMovie();

        public Task Ratemovie(int rating, int movieId);

        public Task<DetailsOutputDto> GetDetails(int id);

        public bool IsUserLiked(int id, string userId);
    }
}

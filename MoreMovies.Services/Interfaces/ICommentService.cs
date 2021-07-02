using MoreMovies.Models;
using MoreMovies.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface ICommentService
    {
        public Task<Comment> AddComment(AddCommentInputModel model, string email);

        public Task Delete();

        public List<Comment> GetMovieComments(int id);
    }
}

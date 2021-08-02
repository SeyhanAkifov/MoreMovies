using MoreMovies.Models;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface ICommentService
    {
        public Task<Comment> AddComment(AddCommentInputModel model);

        public Task Delete();

        public List<CommentOutputDto> GetMovieComments(int id);
    }
}

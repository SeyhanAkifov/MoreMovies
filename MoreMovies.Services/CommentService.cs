using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using MoreMovies.Services.ViewModels.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext db;

        public CommentService(ApplicationDbContext db)
        {
            this.db = db;
        }


        public async Task<Comment> AddComment(AddCommentInputModel model)
        {
            var comment = new Comment 
            { 
                Description = model.Description,
            };
            
            db.Comments.Add(comment);

            await  db.SaveChangesAsync();
            
            return comment;
        }

        public List<string> GetMovieComments(int id)
        {
            var d =  this.db.MovieComments
                .Where(x => x.MovieId == id)
                .Join(this.db.Comments,
                a => a.CommentId,
                b => b.Id,
                (a, b) => b.Description).ToList();
            
            return d;
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }
    }
}

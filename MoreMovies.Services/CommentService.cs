using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class CommentService : ICommentService
    {

        ApplicationDbContext db = new ApplicationDbContext();
        public Comment AddComment(string text)
        {
            
            var comment = new Comment { Description = text };
            

            
                db.Comments.Add(comment);
                db.SaveChanges();

               

                return comment;
            

            
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }
    }
}

using MoreMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface ICommentService
    {
        public Comment AddComment(string text);
        public Task Delete();
    }
}

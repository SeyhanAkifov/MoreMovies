using MoreMovies.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovie.Web.Tests.Mocks.Services
{
    public class CommentServiceMock
    {
        public static CommentService Instance
        {
            get
            {
                var commentService = new CommentService(DatabaseMock.Instance);

                return commentService;
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.ViewModels.Movie
{
    public class AddCommentInputModel
    {
        public string Description { get; set; }
        public string UserId { get; set; }
        public string movieId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Models
{
    public class MovieComments
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        public int CommentId { get; set; }

        public virtual Comment Comment { get; set; }
    }
}

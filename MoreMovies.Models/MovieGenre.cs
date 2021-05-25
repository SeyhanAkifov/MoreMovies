using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Models
{
    public class MovieGenre
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        public int GenreId { get; set; }

        public virtual Genre Genre { get; set; }
    }
}

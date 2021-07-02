using System.Collections.Generic;

namespace MoreMovies.Web.Models
{
    public class MovieListViewModel
    {
        public ICollection<MovieViewModel> TopLiked { get; set; }

        public ICollection<MovieViewModel> TopCommented { get; set; }

        public ICollection<MovieViewModel> Newest { get; set; }
    }
}

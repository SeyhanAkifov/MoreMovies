using MoreMovies.Models;
using System.Collections.Generic;

namespace MoreMovies.Web.Models
{
    public class MovieListViewModel
    {
        public ICollection<MovieViewModel> TopLiked { get; set; }

        public ICollection<MovieViewModel> TopCommented { get; set; }

        public ICollection<MovieViewModel> Newest { get; set; }

        public ICollection<ComingSoonViewModel> ComingSoon { get; set; }

        public ICollection<NewsViewModel> News { get; set; }

        public ICollection<Genre> Genres { get; set; }
    }
}

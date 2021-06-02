using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Models
{
    public class Movie
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int? Likes { get; set; }

        public DateTime ReleaseDate { get; set; }

        public decimal Budget { get; set; }

        public string HomePage { get; set; }

        public int? Popularity { get; set; }

        public TimeSpan? Runtime { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<MovieComment> Comments { get; set; }
        public virtual ICollection<MovieActor> Actors { get; set; }

        public virtual MovieLanguage  Language { get; set; }
        public virtual MovieGenre  Genre { get; set; }
        public virtual MovieCountry  Country { get; set; }
    }
}

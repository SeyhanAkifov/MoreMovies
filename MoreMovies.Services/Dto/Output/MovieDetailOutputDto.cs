using MoreMovies.Models;
using System.Collections.Generic;

namespace MoreMovies.Services.Dto.Output
{
    public class MovieDetailOutputDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Likes { get; set; }

        public bool IsUserLiked { get; set; }

        public string Description { get; set; }

        public string Creator { get; set; }

        public string HomePage { get; set; }

        public int Rating { get; set; }

        public int RatingCount { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<MovieComment> Comments { get; set; }

        public virtual ICollection<MovieActor> Actors { get; set; }
    }
}

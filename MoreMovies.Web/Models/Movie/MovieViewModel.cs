using MoreMovies.Models;
using System;
using System.Collections.Generic;

namespace MoreMovies.Web.Models
{
    public class MovieViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public string Language { get; set; }

        public string Country { get; set; }

        public int? Likes { get; set; }

        public DateTime ReleaseDate { get; set; }

        public decimal Budget { get; set; }

        public string HomePage { get; set; }

        public string Creator { get; set; }

        public int? Popularity { get; set; }

        public TimeSpan? Runtime { get; set; }

        public string ImageUrl { get; set; }

        public List<Comment> Comments { get; set; }

        public List<string> Actors { get; set; }
    }
}

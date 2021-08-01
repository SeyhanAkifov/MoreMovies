using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Models
{
    public class Movie
    {
        public Movie()
        {
            this.Comments = new HashSet<MovieComment>();
            this.Actors = new HashSet<MovieActor>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Creator { get; set; }

        [Required]
        [StringLength(MovieMaxTitleLength)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int Likes { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [Range(0, Double.PositiveInfinity)]
        public decimal Budget { get; set; }

        [Required]
        [Url]
        public string HomePage { get; set; }

        [Range(0, 5)]
        public int Rating { get; set; }


        public int RatingCount { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; }

        public virtual ICollection<MovieComment> Comments { get; set; }

        public virtual ICollection<MovieActor> Actors { get; set; }

        public virtual MovieLanguage Language { get; set; }

        public virtual MovieGenre Genre { get; set; }

        public virtual MovieCountry Country { get; set; }
    }
}

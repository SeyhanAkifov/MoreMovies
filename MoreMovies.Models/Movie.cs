using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public string CreatorId { get; set; }

        [Required]
        [StringLength(30)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int? Likes { get; set; } = 0;

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public decimal Budget { get; set; }

        [Required]
        public string HomePage { get; set; }

        public int? Popularity { get; set; }

        public TimeSpan? Runtime { get; set; }

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

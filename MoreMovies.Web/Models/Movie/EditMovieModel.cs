using System.ComponentModel.DataAnnotations;
using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Web.Models.Movie
{
    public class EditMovieModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(MovieMaxTitleLength, MinimumLength = MovieMinTitleLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(MovieMaxDescriptionLength, MinimumLength = MovieMinDescriptionLength)]
        public string Description { get; set; }

        [Required]
        [Url]
        public string HomePage { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; }

    }
}

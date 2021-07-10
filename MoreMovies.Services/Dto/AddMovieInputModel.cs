using System.ComponentModel.DataAnnotations;

using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Services.Dto
{
    public class AddMovieInputModel
    {
        public string UserId { get; set; }

        [Required]
        [StringLength(MovieMaxTitleLength), MinLength(MovieMinTitleLength)]
        public string Title { get; set; }

        [Required]
        
        public string Description { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        [Url]
        public string Image { get; set; }

        [Required]
        [StringLength(MovieMaxHomePageLength), MinLength(MovieMinHomePageLength)]
        public string HomePage { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string ReleaseDate { get; set; }

        [Required]
        [Range(0, double.PositiveInfinity)]
        public decimal Budget { get; set; }

        [Required]
        public string Actors { get; set; }
    }
}

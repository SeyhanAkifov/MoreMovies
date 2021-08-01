using System.ComponentModel.DataAnnotations;

using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Services.Dto.Input
{
    public class NewsAddModel
    {
        [Required]
        [StringLength(NewsMaxTitleLength, MinimumLength = NewsMinTitleLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(NewsMaxDescriptionLength, MinimumLength = NewsMinDescriptionLength)]
        public string Description { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Services.Dto
{
    public class ComingSoonAddModel
    {
        [Required]
        [StringLength(ComingSoonMaxTitleLength, MinimumLength = ComingSoonMinTitleLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(ComingSoonMaxDescriptionLength, MinimumLength = ComingSoonMinDescriptionLength)]
        public string Description { get; set; }

        [Required]
        
        public string ImageUrl { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AddedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(NewsMaxTitleLength)]
        public string Title { get; set; }

        [Required]
        [StringLength(NewsMaxDescriptionLength)]
        public string Description { get; set; }
    }
}

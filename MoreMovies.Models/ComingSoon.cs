using System;
using System.ComponentModel.DataAnnotations;

namespace MoreMovies.Models
{
    public class ComingSoon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AddedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ImageUrl { get; set; }
    }
}

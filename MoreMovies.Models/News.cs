using System;
using System.ComponentModel.DataAnnotations;

namespace MoreMovies.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AddedOn { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(60)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace MoreMovies.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserFullName { get; set; }
    }
}

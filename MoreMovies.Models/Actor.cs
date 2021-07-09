using System.ComponentModel.DataAnnotations;

namespace MoreMovies.Models
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Name { get; set; }
    }
}

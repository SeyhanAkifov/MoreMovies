using System.ComponentModel.DataAnnotations;

namespace MoreMovies.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [StringLength(30)]
        public string Name { get; set; }
    }
}

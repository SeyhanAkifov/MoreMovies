using System.ComponentModel.DataAnnotations;

namespace MoreMovies.Models
{
    public class CinemaHall
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

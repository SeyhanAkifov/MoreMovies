using System.ComponentModel.DataAnnotations;

namespace MoreMovies.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(5)]
        public string CountryCode { get; set; }
    }
}

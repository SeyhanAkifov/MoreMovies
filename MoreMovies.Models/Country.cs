using System.ComponentModel.DataAnnotations;

using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(CountryMaxNameLength)]
        public string Name { get; set; }

        [StringLength(CountryMaxCodeLength)]
        public string CountryCode { get; set; }
    }
}

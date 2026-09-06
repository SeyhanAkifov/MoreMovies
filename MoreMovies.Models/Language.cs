using System.ComponentModel.DataAnnotations;

using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Models
{
    public class Language
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(LanguageMaxNameLength, MinimumLength = LanguageMinNameLength)]
        public string Name { get; set; }
    }
}

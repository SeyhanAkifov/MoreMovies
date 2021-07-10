using System.ComponentModel.DataAnnotations;

using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Models
{
    
    public class Actor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(ActorMaxNameLength)]
        public string Name { get; set; }
    }
}

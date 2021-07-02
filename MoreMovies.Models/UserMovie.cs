using Microsoft.AspNetCore.Identity;

namespace MoreMovies.Models
{
    public class UserMovie
    {
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public int MovieId { get; set; }
        public Movie Movies { get; set; }
    }
}

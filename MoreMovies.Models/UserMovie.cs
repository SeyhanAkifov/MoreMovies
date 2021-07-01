using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MoreMovies.Models
{
    public class ApplicationUser : IdentityUser
    {


        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }

        public ICollection<UserMovie> Movies { get; set; }

    }
}

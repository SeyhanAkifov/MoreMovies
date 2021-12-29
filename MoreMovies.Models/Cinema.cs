using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Models
{
    public class Cinema
    {
        public Cinema()
        {
            this.CinemaPojections = new HashSet<CinemaPojection>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; }

        public IdentityUser User { get; set; }


        public ICollection<CinemaPojection> CinemaPojections { get; set; }
    }
}

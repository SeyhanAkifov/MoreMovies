using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Dto.Input
{
    public class CinemaInputDto
    {
        [Required]
        [StringLength(25)]
        public string CinemaName { get; set; }
    }
}

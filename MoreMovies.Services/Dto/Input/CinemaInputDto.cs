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
        public string CinemaName { get; set; }
    }
}

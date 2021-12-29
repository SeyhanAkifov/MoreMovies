using MoreMovies.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Dto.Input
{
    public class CinemaProjectionInputDto
    {
        [Required]
        public DateTime ProjectionTime { get; set; }

        [Required]
        public string MovieName { get; set; }

        [Required]
        public string CinemaHallName { get; set; }

        [Required]
        public string Time { get; set; }

        [Required]
        public string CinemaName { get; set; }


    }
}

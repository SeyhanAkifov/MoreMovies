using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Models
{
    public class CinemaPojection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ProjectionTime { get; set; }

        [Required]
        public string MovieName { get; set; }

        [Required]
        public string Time { get; set; }
        
        [Required]
        public int CinemaHallId { get; set; }
        
        public CinemaHall CinemaHall { get; set; }
    }
}

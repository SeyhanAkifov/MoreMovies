using MoreMovies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Dto.Output
{
    public class CinemaProjectionOutputDto
    {
        public int Id { get; set; }
        
        public DateTime ProjectionTime { get; set; }
        
        public string MovieName { get; set; }

        public string Time { get; set; }
        
        public CinemaHall CinemaHall { get; set; }

        
    }
}

using System;

namespace MoreMovies.Web.Models.Cinema
{
    public class CinemaProjectionViewModel
    {
        public int Id { get; set; }

        public DateTime ProjectionTime { get; set; }

        public string MovieName { get; set; }

        public string CinemaHall { get; set; }

        public string Time { get; set; }
    }
}

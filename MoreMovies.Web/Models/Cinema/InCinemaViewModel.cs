using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Web.Models.Cinema
{
    public class InCinemaViewModel
    {
        public List<string> CinemaNames { get; set; }

        public ICollection<CinemaProjectionViewModel> Projections { get; set; }
    }
}

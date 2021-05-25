using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Models
{
    public class MovieCountry
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}

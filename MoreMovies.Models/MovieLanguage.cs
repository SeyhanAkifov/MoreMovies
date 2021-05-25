using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Models
{
    public class MovieLanguage
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        public int LanguageId { get; set; }

        public virtual Language Language { get; set; }

    }
}

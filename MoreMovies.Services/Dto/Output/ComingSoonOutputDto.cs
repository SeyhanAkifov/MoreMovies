using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Dto.Output
{
    public class ComingSoonOutputDto
    {
        public int Id { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }
}

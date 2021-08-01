﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Dto.Output
{
    public class DetailsOutputDto
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public string Language { get; set; }

        public string Country { get; set; }

        public DateTime ReleaseDate { get; set; }

        public decimal Budget { get; set; }

        public string HomePage { get; set; }
    }


}

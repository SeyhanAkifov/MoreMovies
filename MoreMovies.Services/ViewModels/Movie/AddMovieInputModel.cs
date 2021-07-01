﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.ViewModels.Movie
{
    public class AddMovieInputModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public string Image { get; set; }

        public string HomePage { get; set; }

        public string Country { get; set; }

        public string Language { get; set; }


        public string ReleaseDate { get; set; }

        public decimal Budget { get; set; }
        public string Actors { get; set; }
    }       
}

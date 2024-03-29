﻿using System.ComponentModel.DataAnnotations;

using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }


        [Required]
        [StringLength(GenreMaxNameLength)]
        public string Name { get; set; }
    }
}

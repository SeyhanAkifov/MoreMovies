﻿using System;

namespace MoreMovies.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UserFullName { get; set; }
    }
}

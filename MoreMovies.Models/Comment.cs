using System;

namespace MoreMovies.Models
{
    public class Comment
    {
        [key]
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string UserFullName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}

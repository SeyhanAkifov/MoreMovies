using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        public DateTime AddedOn { get; set; } = DateTime.UtcNow;

        public string Title { get; set; }

        public string Description { get; set; }
    }
}

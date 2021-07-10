using System.ComponentModel.DataAnnotations;

namespace MoreMovies.Services.Dto
{
    public class AddCommentInputModel
    {
        [Required]
        public string Description { get; set; }

        public string UserId { get; set; }

        public string MovieId { get; set; }
    }
}

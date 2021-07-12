using System.ComponentModel.DataAnnotations;
using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Services.Dto
{
    public class AddCommentInputModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(CommentDescriptionMaxLength, MinimumLength = CommentDescriptionMinLength)]
        public string Description { get; set; }

        public string UserId { get; set; }

        public int MovieId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using static MoreMovies.Models.DataConstants;

namespace MoreMovies.Services.Dto.Input
{
    public class AddCommentInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please write something before submitting.")]
        [StringLength(CommentDescriptionMaxLength, MinimumLength = CommentDescriptionMinLength,
            ErrorMessage = "A comment can be at most {1} characters long.")]
        public string Description { get; set; }

        public string UserId { get; set; }

        public int MovieId { get; set; }
    }
}

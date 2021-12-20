using System.ComponentModel.DataAnnotations;

namespace MoreMovies.Services.Dto.Input
{
    public class EditMovieInputModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string HomePage { get; set; }
        [Required]
        public string ImageUrl { get; set; }
    }
}

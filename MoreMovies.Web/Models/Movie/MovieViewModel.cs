namespace MoreMovies.Web.Models
{
    public class MovieViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Likes { get; set; }

        public string HomePage { get; set; }

        public int Rating { get; set; }

        public int RatingCount { get; set; }

        public string ImageUrl { get; set; }

        public bool IsUserLiked { get; set; }

        public int CommentsCount { get; set; }
    }
}

namespace MoreMovies.Models
{
    public class MovieComment
    {
        public int MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public int CommentId { get; set; }

        public virtual Comment Comment { get; set; }
    }
}

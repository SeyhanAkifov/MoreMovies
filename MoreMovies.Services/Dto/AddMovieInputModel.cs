namespace MoreMovies.Services.Dto
{
    public class AddMovieInputModel
    {
        public string UserId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Genre { get; set; }

        public string Image { get; set; }

        public string HomePage { get; set; }

        public string Country { get; set; }

        public string Language { get; set; }
        
        public string ReleaseDate { get; set; }

        public decimal Budget { get; set; }

        public string Actors { get; set; }
    }
}

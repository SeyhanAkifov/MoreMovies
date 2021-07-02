namespace MoreMovies.Models
{
    public class MovieCountry
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}

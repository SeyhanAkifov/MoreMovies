using MoreMovies.Services;
using MoreMovies.Web.Controllers;

namespace MoreMovie.Web.Tests.Mocks
{
    public static class MovieControllerMock
    {
        public static MovieController Instance
        {
            get
            {
                var data = DatabaseMock.Instance;
                var commentService = new CommentService(data);
                var languageService = new LanguageService(data);
                var countryService = new CountryService(data);
                var genreService = new GenreService(data);
                var newsService = new NewsService(data);
                var actorService = new ActorService(data);
                var movieService = new MovieService(data);

                return new MovieController(movieService, commentService, actorService, languageService, genreService, countryService, null);
            }
            set
            {

            }
        }


    }
}

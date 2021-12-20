using AutoMapper;
using MoreMovies.Services;
using MoreMovies.Web.Controllers;
using MoreMovies.Web.MappingConfiguration;

namespace MoreMovie.Web.Tests.Mocks
{
    public static class MovieControllerMock
    {
        public static MovieController Instance
        {
            get
            {
                var data = DatabaseMock.Instance;

                var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
                var commentService = new CommentService(data);
                var languageService = new LanguageService(data);
                var countryService = new CountryService(data);
                var genreService = new GenreService(data);
                var newsService = new NewsService(data);
                var actorService = new ActorService(data);
                var movieService = new MovieService(commentService, data, languageService, genreService, countryService, mapper);

                return new MovieController(movieService, mapper, commentService, actorService, languageService, genreService, countryService, null);
            }
            set
            {

            }
        }


    }
}

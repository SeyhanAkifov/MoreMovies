using AutoMapper;
using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Services;
using MoreMovies.Web.MappingConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests.Service
{
    public class MovieServiceTests
    {
        [Fact]
        public async void Test()
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

            //Act
            var newstMovies = await movieService.GetNewestAddedAllMovie();
            var allMovies = data.Movies.OrderByDescending(x => x.ReleaseDate);

            var newestFirst = newstMovies.First();
            var allFirst = newstMovies.First();

            //Assert
            Assert.Equal(newstMovies.Count, allMovies.Count());
            Assert.Equal(newestFirst.Id, allFirst.Id);
        }
    }
}

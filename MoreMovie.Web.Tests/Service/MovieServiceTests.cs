using AutoMapper;
using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Models;
using MoreMovies.Services;
using MoreMovies.Web.MappingConfiguration;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests.Service
{
    public class MovieServiceTests
    {
        [Fact]
        public async Task AllNewestAddedShouldReturnCorrectOrderedList()
        {
            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService);

            //Act
            var newstMovies = await movieService.GetNewestAddedAllMovie();
            var allMovies = data.Movies.OrderByDescending(x => x.ReleaseDate);

            var newestFirst = newstMovies.First();
            var allFirst = allMovies.First();

            //Assert
            Assert.Equal(newstMovies.Count, allMovies.Count());
            Assert.Equal(newestFirst.Id, allFirst.Id);
        }

        [Fact]
        public async Task NewestAddedShouldReturnCorrectOrderedList()
        {
            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService);

            //Act
            var newstMovies = await movieService.GetNewestAddedMovie();
            var expectedCount = 5;

            //Assert
            Assert.Equal(expectedCount, newstMovies.Count);

        }

        [Fact]
        public async Task AllTopCommentedShouldReturnCorrectOrderedList()
        {
            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService);

            //Act
            var topCommentedMovies = await movieService.GetTopCommentedAllMovie();
            var allMovies = data.Movies.OrderByDescending(x => x.Comments.Count);

            var newestFirst = topCommentedMovies.First();
            var allFirst = allMovies.First();

            //Assert
            Assert.Equal(topCommentedMovies.Count, allMovies.Count());
            Assert.Equal(newestFirst.Id, allFirst.Id);
        }

        [Fact]
        public async Task TopCommentedShouldReturnCorrectOrderedList()
        {
            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService);

            //Act
            var topCommentedMovies = await movieService.GetTopCommentedMovie();

            var expectedCount = 5;
            //Assert
            Assert.Equal(expectedCount, topCommentedMovies.Count);

        }

        [Fact]
        public async Task AllTopLikedShouldReturnCorrectOrderedList()
        {
            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService);

            //Act
            var topCommentedMovies = await movieService.GetTopLikedAllMovie();
            var allMovies = data.Movies.OrderByDescending(x => x.Likes);

            var newestFirst = topCommentedMovies.First();
            var allFirst = allMovies.First();

            //Assert
            Assert.Equal(topCommentedMovies.Count, allMovies.Count());
            Assert.Equal(newestFirst.Id, allFirst.Id);
        }

        [Fact]
        public async Task TopLikedShouldReturnCorrectOrderedList()
        {
            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService);

            //Act
            var topCommentedMovies = await movieService.GetTopLikedMovie();

            var expectedCount = 5;
            //Assert
            Assert.Equal(expectedCount, topCommentedMovies.Count);

        }

        [Fact]
        public async Task SearchByGenreShouldReturnCorrectCount()
        {
            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService);

            data.Movies.Add(new Movie
            {
                Creator = "Admin1@abv.bg",
                Title = "Search test movie",
                Description = "Test movie description",
                HomePage = "http://test.com",
                ImageUrl = "http://test.com/image.jpg",
                Genre = new MovieGenre { Genre = new Genre { Name = "Action" } }
            });

            data.SaveChanges();
            //Act
            var result = await movieService.SearchMovieByGenre("Action");

            //Assert
            Assert.Equal(1, result.Count);

        }

        [Fact]
        public async Task SearchByYearShouldReturnCorrectCount()
        {
            var data = DatabaseMock.Instance;

            var mapper = new Mapper(new MapperConfiguration(config => config.AddProfile(new ApplicationProfile())));
            var commentService = new CommentService(data);
            var languageService = new LanguageService(data);
            var countryService = new CountryService(data);
            var genreService = new GenreService(data);
            var newsService = new NewsService(data);
            var actorService = new ActorService(data);
            var movieService = new MovieService(commentService, data, languageService, genreService, countryService);

            data.Movies.Add(new Movie
            {
                Creator = "Admin1@abv.bg",
                Title = "Search test movie",
                Description = "Test movie description",
                HomePage = "http://test.com",
                ImageUrl = "http://test.com/image.jpg",
                ReleaseDate = DateTime.ParseExact("01/01/1983", "dd/MM/yyyy", CultureInfo.InvariantCulture),
            });

            data.SaveChanges();
            //Act
            var result = await movieService.SearchMovieByYear("1983");

            //Assert
            Assert.Equal(1, result.Count);


        }
    }
}

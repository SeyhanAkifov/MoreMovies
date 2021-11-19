using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace MoreMovies.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext db;
        private readonly ICommentService commentService;
        private readonly ILanguageService languageService;
        private readonly IGenreService genreService;
        private readonly ICountryService countryService;

        public MovieService(ICommentService commentService, ApplicationDbContext db, ILanguageService languageService, IGenreService genreService, ICountryService countryService, IMapper mapper)
        {
            this.commentService = commentService;
            this.languageService = languageService;
            this.genreService = genreService;
            this.countryService = countryService;
            this.db = db;
        }

        public async Task AddMovie(AddMovieInputModel model)
        {

            var language = db.Languages.FirstOrDefault(x => x.Name == model.Language);


            var genre = db.Genre.FirstOrDefault(x => x.Name == model.Genre);

            

            var country = db.Country.FirstOrDefault(x => x.Name == model.Country);

            

            var editedHomePage = model.HomePage.Replace("watch?v=", "embed/");

            Movie movie = new()
            {
                Title = model.Title,
                Budget = model.Budget,
                ReleaseDate = DateTime.ParseExact(model.ReleaseDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                Description = model.Description,
                HomePage = editedHomePage,
                Language = new MovieLanguage { Language = language },
                Genre = new MovieGenre { Genre = genre },
                Country = new MovieCountry { Country = country },
                ImageUrl = model.Image,
                Creator = model.Creator

            };
            string[] actorNames = model.Actors.Split(", ");

            foreach (var name in actorNames)
            {
                if (!this.db.Actors.Any(x => x.Name == name))
                {
                    var actor = new Actor()
                    {
                        Name = name
                    };

                    this.db.Actors.Add(actor);

                    movie.Actors.Add(new MovieActor { Actor = actor });
                }
                else
                {
                    var actor = await this.db.Actors.FirstOrDefaultAsync(x => x.Name == name);
                    movie.Actors.Add(new MovieActor { Actor = actor });
                }
            }

            var result = db.Movies.Add(movie);

            db.SaveChanges();
        }

        public async Task DeleteMovie(int id)
        {
            var movie = await this.db.Movies.FindAsync(id);

            db.Movies.Remove(movie);

            await db.SaveChangesAsync();
        }

        public async Task EditMovieWithId(int id, EditMovieInputModel model)
        {
            var movie = await this.db.Movies.FindAsync(id);

            movie.Title = model.Title;
            movie.HomePage = model.HomePage;
            movie.ImageUrl = model.ImageUrl;
            movie.Description = model.Description;

            await db.SaveChangesAsync();
        }

        public async Task LikeMovie(int id, string userId)
        {
            var movie = db.Movies.Find(id);
            
            movie.Likes++;

            this.db.UserMovies.Add(new UserMovie { MovieId = movie.Id, UserId = userId });

            await db.SaveChangesAsync();
        }

        public async Task AddComment(AddCommentInputModel model)
        {
            var movie = await GetMovieWithId(model.MovieId);

            var comment = await commentService.AddComment(model);

            movie.Comments.Add(new MovieComment { Comment = comment });

            await db.SaveChangesAsync();
        }

        public async Task<ICollection<MovieOutputDto>> GetAllMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .Select(x => GetMovieOutputDto(x))
                .ToArrayAsync();

            return movies;
        }
        
        public async Task<ICollection<MovieOutputDto>> GetAllMyMovie(string userName)
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Where(x => x.Creator == userName)
                .Select(x => GetMovieOutputDto(x))
                .ToArrayAsync();

            return movies;
        }

        public async Task<MovieDetailOutputDto> GetMovieWithId(int id)
        {
            Movie movie = await db.Movies.FindAsync(id);

            return GetMovieDetailOutputDto(movie);
        }

        

        public async Task<ICollection<MovieOutputDto>> GetTopCommentedMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Comments.Count)
                .Take(6)
                .Select(x => GetMovieOutputDto(x))
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> GetTopCommentedAllMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Comments.Count)
                .Select(x => GetMovieOutputDto(x))
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> GetTopLikedMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Likes)
                .Take(6)
                .Select(x => GetMovieOutputDto(x))
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> GetTopLikedAllMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Likes)
                .Select(x => GetMovieOutputDto(x))
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> GetNewestAddedMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.ReleaseDate)
                .Take(6)
                .Select(x => GetMovieOutputDto(x))
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> GetNewestAddedAllMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => GetMovieOutputDto(x))
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> SearchMovie(string name)
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                            .Include(x => x.Genre.Genre)
                            .Include(x => x.Language.Language)
                            .Include(x => x.Country.Country)
                            .Include(x => x.Comments)
                            .Where(x => x.Title.ToLower().Contains(name.ToLower()))
                            .Select(x => GetMovieOutputDto(x))
                            .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> SearchMovieByGenre(string genre)
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .Where(x => x.Genre.Genre.Name == genre)
                .Select(x => GetMovieOutputDto(x))
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> SearchMovieByYear(string year)
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .Where(x => x.ReleaseDate.Year == int.Parse(year))
                .Select(x => GetMovieOutputDto(x))
                .ToArrayAsync();

            return movies;
        }

        public async Task Ratemovie(int rating, int movieId)
        {
            var movie = await this.db.Movies.FindAsync(movieId);
            movie.Rating += rating;
            movie.RatingCount++;

            await this.db.SaveChangesAsync();
        }

        public async Task<DetailsOutputDto> GetDetails(int id)
        {
            DetailsOutputDto movie = await db.Movies
                .Select(x => new DetailsOutputDto
                {
                    Id = x.Id,
                    Description = x.Description,
                    ReleaseDate = x.ReleaseDate,
                    Budget = x.Budget,
                    Language = x.Language.Language.Name,
                    Country = x.Country.Country.Name,
                    Genre = x.Genre.Genre.Name,
                    HomePage = x.HomePage,

                }).FirstOrDefaultAsync(x => x.Id == id);

            return movie;
        }

        public bool IsUserLiked(int id, string userId)
        {
            return this.db.UserMovies.Any(x => x.MovieId == id && x.UserId == userId);

        }

        public async Task<ICollection<MovieOutputDto>> GetAllMyLiked(string userId)
        {
            ICollection<MovieOutputDto> movies = await db.UserMovies.Where(um => um.UserId == userId)
                 .Select(x => x.Movies)
                 .Select(x => GetMovieOutputDto(x))
                 .ToArrayAsync();

            return movies;
        }

        private static MovieOutputDto GetMovieOutputDto(Movie movie)
        {
            return new MovieOutputDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Likes = movie.Likes,
                Rating = movie.Rating,
                RatingCount = movie.RatingCount,
                ImageUrl = movie.ImageUrl,
                CommentsCount = movie.Comments.Count,
                HomePage = movie.HomePage,
            };
        }

        private static MovieDetailOutputDto GetMovieDetailOutputDto(Movie movie)
        {
            return new MovieDetailOutputDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Likes = movie.Likes,
                Description = movie.Description,
                IsUserLiked = movie.IsUserLiked,
                Rating = movie.Rating,
                RatingCount = movie.RatingCount,
                ImageUrl = movie.ImageUrl,
                Comments = movie.Comments,
                Actors = movie.Actors,
                HomePage = movie.HomePage,
                Creator = movie.Creator
            };
        }
    }
}

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
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace MoreMovies.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext db;
        public MovieService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task AddMovie(AddMovieInputModel model)
        {
            var language = await db.Languages.FirstOrDefaultAsync(x => x.Name == model.Language);
            var genre = await db.Genre.FirstOrDefaultAsync(x => x.Name == model.Genre);
            var country = await db.Country.FirstOrDefaultAsync(x => x.Name == model.Country);

            var editedHomePage = model.HomePage.Replace("watch?v=", "embed/");

            Movie movie = new()
            {
                Title = model.Title,
                Budget = model.Budget,
                ReleaseDate = DateTime.ParseExact(model.ReleaseDate, AddMovieInputModel.ReleaseDateFormat, CultureInfo.InvariantCulture),
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
                var actor = await db.Actors.FirstOrDefaultAsync(x => x.Name == name);
                movie.Actors.Add(new MovieActor { Actor = actor ?? new Actor { Name = name } });
            }

            db.Movies.Add(movie);
            await db.SaveChangesAsync();
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
            var movie = await db.Movies.FindAsync(id);
            
            movie.Likes++;

            this.db.UserMovies.Add(new UserMovie { MovieId = movie.Id, UserId = userId });

            await db.SaveChangesAsync();
        }

        public async Task DisLikeMovie(int id, string userId)
        {
            var movie = await db.Movies.FindAsync(id);

            movie.Likes--;

            var usermovie = await db.UserMovies.FirstOrDefaultAsync(x => x.MovieId == movie.Id && x.UserId == userId);

            this.db.UserMovies.Remove(usermovie);

            await db.SaveChangesAsync();
        }


        public async Task<ICollection<MovieOutputDto>> GetAllMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Select(MovieProjection)
                .ToArrayAsync();

            return movies;
        }
        
        public async Task<ICollection<MovieOutputDto>> GetAllMyMovie(string userName)
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Where(x => x.Creator == userName)
                .Select(MovieProjection)
                .ToArrayAsync();

            return movies;
        }

        public async Task<MovieDetailOutputDto> GetMovieWithId(int id)
        {
            Movie movie = await db.Movies.FindAsync(id);

            return movie == null ? null : GetMovieDetailOutputDto(movie);
        }

        public async Task<ICollection<MovieOutputDto>> GetTopCommentedMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .OrderByDescending(x => x.Comments.Count)
                .Take(5)
                .Select(MovieProjection)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> GetTopCommentedAllMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .OrderByDescending(x => x.Comments.Count)
                .Select(MovieProjection)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> GetTopLikedMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .OrderByDescending(x => x.Likes)
                .Take(5)
                .Select(MovieProjection)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> GetTopLikedAllMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .OrderByDescending(x => x.Likes)
                .Select(MovieProjection)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> GetNewestAddedMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .OrderByDescending(x => x.ReleaseDate)
                .Take(5)
                .Select(MovieProjection)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> GetNewestAddedAllMovie()
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .OrderByDescending(x => x.ReleaseDate)
                .Select(MovieProjection)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> SearchMovie(string name)
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                            .Where(x => x.Title.ToLower().Contains(name.ToLower()))
                            .Select(MovieProjection)
                            .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> SearchMovieByGenre(string genre)
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Where(x => x.Genre.Genre.Name == genre)
                .Select(MovieProjection)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<MovieOutputDto>> SearchMovieByYear(string year)
        {
            ICollection<MovieOutputDto> movies = await db.Movies
                .Where(x => x.ReleaseDate.Year == int.Parse(year))
                .Select(MovieProjection)
                .ToArrayAsync();

            return movies;
        }

        public async Task<bool> RateMovie(int rating, int movieId)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(rating, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(rating, 5);

            if (db.Database.IsRelational())
            {
                var updated = await db.Movies.Where(movie => movie.Id == movieId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(movie => movie.Rating, movie => movie.Rating + rating)
                        .SetProperty(movie => movie.RatingCount, movie => movie.RatingCount + 1));
                return updated > 0;
            }

            var movie = await db.Movies.FindAsync(movieId);
            if (movie is null)
            {
                return false;
            }

            movie.Rating += rating;
            movie.RatingCount++;

            await db.SaveChangesAsync();
            return true;
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
                 .Select(MovieProjection)
                 .ToArrayAsync();

            return movies;
        }

        private static readonly Expression<Func<Movie, MovieOutputDto>> MovieProjection = movie => new MovieOutputDto
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

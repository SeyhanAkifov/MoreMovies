using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto;
using MoreMovies.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
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
        private readonly UserManager<IdentityUser> userManager;
        private readonly IdentityUser user;

        public MovieService(IdentityUser user, UserManager<IdentityUser> userManager, ICommentService commentService, ApplicationDbContext db, ILanguageService languageService, IGenreService genreService, ICountryService countryService)
        {
            this.commentService = commentService;
            this.languageService = languageService;
            this.genreService = genreService;
            this.countryService = countryService;
            this.db = db;
            this.userManager = userManager;
            this.user = user;
        }

        public async Task AddMovie(AddMovieInputModel model)
        {
            var user = await userManager.FindByEmailAsync(model.UserId);
            var language = db.Languages.FirstOrDefault(x => x.Name == model.Language);

            if (language == null)
            {
                await languageService.Add(model.Language);

                language = db.Languages.FirstOrDefault(x => x.Name == model.Language);
            }

            var genre = db.Genre.FirstOrDefault(x => x.Name == model.Genre);

            if (genre == null)
            {
                await genreService.Add(model.Genre);

                genre = db.Genre.FirstOrDefault(x => x.Name == model.Genre);
            }

            var country = db.Country.FirstOrDefault(x => x.Name == model.Country);

            if (country == null)
            {
                await countryService.Add(model.Country);

                country = db.Country.FirstOrDefault(x => x.Name == model.Country);
            }

            

            Movie movie = new()
            {
                Title = model.Title,
                Budget = model.Budget,
                ReleaseDate = DateTime.ParseExact(model.ReleaseDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                Runtime = new TimeSpan(1, 36, 50),
                Description = model.Description,
                HomePage = model.HomePage,
                Language = new MovieLanguage { Language = language },
                Genre = new MovieGenre { Genre = genre },
                Country = new MovieCountry { Country = country },
                ImageUrl = model.Image,
                Creator = model.UserId

            };
            string[] actorNames = model.Actors.Split(", ");

            foreach (var name in actorNames)
            {
                var actor = new Actor()
                {
                    Name = name
                };

                if (this.db.Actors.Any(x => x.Name == name))
                {
                    movie.Actors.Add(new MovieActor { Actor = actor });
                    continue;
                }
                this.db.Actors.Add(actor);
                movie.Actors.Add(new MovieActor { Actor = actor });
            }
                var userMovie = new UserMovie()
                {
                    Movies = movie,
                    User = user
                };
            

            var result = db.Movies.Add(movie);

            this.db.UserMovies.Add(userMovie);

            db.SaveChanges();
        }

        public async Task DeleteMovie(int id)
        {
            var movie = await this.db.Movies.FindAsync(id);

            db.Movies.Remove(movie);

            await db.SaveChangesAsync();
        }

        public async Task EditMovieWithId(int id, AddMovieInputModel model)
        {
            var movie = await this.db.Movies.FindAsync(id);

            movie.Title = model.Title;
            movie.HomePage = model.HomePage;
            movie.ImageUrl = model.Image;
            movie.Description = model.Description;

            await db.SaveChangesAsync();
        }

        public async Task LikeMovie(int id)
        {
            var movie = db.Movies.Find(id);

            movie.Likes++;

            await db.SaveChangesAsync();
        }

        public async Task AddComment(AddCommentInputModel model)
        {
            var movie = await GetMovieWithId(model.MovieId);
            
            var comment = await commentService.AddComment(model);

            movie.Comments.Add(new MovieComment { Comment = comment });

            await db.SaveChangesAsync();
        }

        public async Task<ICollection<Movie>> GetAllMovie()
        {

            ICollection<Movie> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                //.Take(6)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<Movie>> GetAllMyMovie(string userId)
        {
           

            ICollection<Movie> movies = await db.UserMovies.Where(um => um.UserId == userId)
                .Select(x => x.Movies)
                .ToArrayAsync();

            return movies;
        }

        public async Task<Movie> GetMovieWithId(int id)
        {
            Movie movie = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .Include(x => x.Actors)
                .FirstAsync(x => x.Id == id);

            return movie;
        }

        public async Task<int> SearchMovie(string name)
        {

            var movie = await this.db.Movies.FirstOrDefaultAsync(x => x.Title.ToLower() == name.ToLower());

            return movie != null ? movie.Id : 0;
        }

        public async Task<ICollection<Movie>> GetTopCommentedMovie()
        {
            ICollection<Movie> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Comments.Count)
                .Take(6)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<Movie>> GetTopCommentedAllMovie()
        {
            ICollection<Movie> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Comments.Count)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<Movie>> GetTopLikedMovie()
        {
            ICollection<Movie> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Likes)
                .Take(6)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<Movie>> GetTopLikedAllMovie()
        {
            ICollection<Movie> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.Likes)
                .ToArrayAsync();

            return movies;
        }
        
        public async Task<ICollection<Movie>> GetNewestAddedMovie()
        {
            ICollection<Movie> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.ReleaseDate)
                .Take(6)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<Movie>> GetNewestAddedAllMovie()
        {
            ICollection<Movie> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .OrderByDescending(x => x.ReleaseDate)
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<Movie>> SearchMovieByGenre(string genre)
        {
            ICollection<Movie> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .Where(x => x.Genre.Genre.Name == genre)
                .Take(6)
                .ToArrayAsync();

            return movies;
        }
    }
}

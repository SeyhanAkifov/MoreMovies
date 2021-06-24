using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using MoreMovies.Services.ViewModels.Movie;
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

        public MovieService(ICommentService commentService, ApplicationDbContext db)
        {
            this.commentService = commentService;
            this.db = db;
        }

        public void AddMovie(AddMovieInputModel model)
        {

            var language = db.Languages.FirstOrDefault(x => x.Name == model.Language);

            if (language == null)
            {
                db.Languages.Add(new Language { Name = model.Language });
                db.SaveChanges();
                language = db.Languages.FirstOrDefault(x => x.Name == model.Language);
            }
            var genre = db.Genre.FirstOrDefault(x => x.Name == model.Genre);
            if (genre == null)
            {
                db.Genre.Add(new Genre { Name = model.Genre });
                db.SaveChanges();
                genre = db.Genre.FirstOrDefault(x => x.Name == model.Genre);
            }
            var country = db.Country.FirstOrDefault(x => x.Name == model.Country);
            if (country == null)
            {
                db.Country.Add(new Country { Name = model.Country });
                db.SaveChanges();
                country = db.Country.FirstOrDefault(x => x.Name == model.Country);
            }


            string[] actorNames = new string[] { "John Deep", "Angi Joli" };

            Movie movie = new Movie
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
                ImageUrl = model.Image
            };

            var result = db.Movies.Add(movie);

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

        public async Task AddComment(int movieId, AddCommentInputModel model)
        {
            var movie = await GetMovieWithId(movieId);

            var comment = await commentService.AddComment(model);

            movie.Comments.Add(new MovieComment { Comment = comment });

            await db.SaveChangesAsync();
        }

        public ICollection<Movie> GetAllMovie()
        {
            ICollection<Movie> movies = db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                //.Take(6)
                .ToArray();

            return movies;
        }

        public async Task<Movie> GetMovieWithId(int id)
        {
            Movie movie = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .FirstAsync(x => x.Id == id);
            
            return movie;
        }

        public async Task<int> SearchMovie(string name)
        {
            
            var movie = await this.db.Movies.FirstOrDefaultAsync(x => x.Title.ToLower() == name.ToLower());

            return movie != null ? movie.Id : 0 ;
        }
    }
}

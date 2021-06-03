using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using MoreMovies.Services.ViewModels.Movie;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class MovieService : IMovieService
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public void AddMovie(AddMovieInputModel model)
        {

            var language = db.Languages.FirstOrDefault(x => x.Name == model.Language);

            if (language == null)
            {
                db.Languages.Add(new Language { Name = model.Language });
                db.SaveChanges();
            }
            var genre = db.Genre.FirstOrDefault(x => x.Name == model.Genre);
            if (genre == null)
            {
                db.Genre.Add(new Genre { Name = model.Genre });
                db.SaveChanges();
            }
            var country = db.Country.FirstOrDefault(x => x.Name == model.Country);
            if (country == null)
            {
                db.Country.Add(new Country { Name = model.Country });
                db.SaveChanges();
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
                Language = new MovieLanguage{ Language = language },
                Genre = new MovieGenre { Genre = genre},
                Country = new MovieCountry { Country = country},
                ImageUrl = model.Image



            };
            var result = db.Movies.Add(movie);
            db.SaveChanges();

            // return result;

        }

        public void DeleteMovie(int id)
        {
            var db = new ApplicationDbContext();
            var movie = db.Movies.Find(id);
            db.Movies.Remove(movie);
            db.SaveChanges();
        }

        public void EditMovie()
        {
            throw new NotImplementedException();
        }

        public ICollection<Movie> GetAllMovie()
        {

            var db = new ApplicationDbContext();
            ICollection<Movie> movies = db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .ToArray();

            return movies;
        }

        public Movie GetMovieWithId(int id)
        {
            Movie movie = db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country).First(x => x.Id == id);

            return movie;
        }

        public void SearchMovie()
        {
            throw new NotImplementedException();
        }
    }
}

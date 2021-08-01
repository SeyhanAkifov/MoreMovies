using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto;
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
            //var user = await userManager.FindByEmailAsync(model.Creator);
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
            //var userMovie = new UserMovie()
            //{
            //    Movies = movie,
            //    User = user
            //};


            var result = db.Movies.Add(movie);

            //this.db.UserMovies.Add(userMovie);

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

        public async Task<MovieOutputDto> GetMovieWithId(int id)
        {
            Movie movie = await db.Movies.FindAsync(id);

                var result  = new MovieOutputDto 
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Creator = movie.Creator,
                    Likes = movie.Likes,
                    Rating = movie.Rating,
                    RatingCount = movie.RatingCount,
                    ImageUrl = movie.ImageUrl,
                    Comments = movie.Comments,
                    Actors = movie.Actors,
                    HomePage = movie.HomePage
                };


            return result;
        }

        public async Task<int> SearchMovie(string name)
        {

            var movie = await this.db.Movies.FirstOrDefaultAsync(x => x.Title.Contains(name));

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
                .ToArrayAsync();

            return movies;
        }

        public async Task<ICollection<Movie>> SearchMovieByYear(string year)
        {
            ICollection<Movie> movies = await db.Movies
                .Include(x => x.Genre.Genre)
                .Include(x => x.Language.Language)
                .Include(x => x.Country.Country)
                .Include(x => x.Comments)
                .Where(x => x.ReleaseDate.Year == int.Parse(year))
                .ToArrayAsync();

            return movies;
        }

        public async Task Ratemovie(int rating, int movieId)
        {
            var movie = await this.db.Movies.FindAsync(movieId);
            movie.Rating = rating;
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
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Web.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        // Runs schema migrations for every environment, but only seeds demo
        // content/accounts (Movies/News/ComingSoon/Admin1/User1) in Development,
        // so a fresh production database never ships with well-known credentials.
        public static async Task PrepareDatabaseAsync(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();
            var services = scopedServices.ServiceProvider;

            var db = services.GetRequiredService<ApplicationDbContext>();
            var ms = services.GetRequiredService<IMovieService>();
            var ns = services.GetRequiredService<INewsService>();
            var cs = services.GetRequiredService<IComingSoonService>();

            if (db.Database.IsRelational())
            {
                await db.Database.MigrateAsync();
            }
            else
            {
                await db.Database.EnsureCreatedAsync();
            }

            await SeedGenre(db);
            await SeedLanguage(db);
            await SeedCountry(db);

            if (env.IsDevelopment())
            {
                await SeedUsers(services);
                await SeedMovies(db, ms, env.ContentRootPath);
                await SeedNews(db, ns, env.ContentRootPath);
                await SeedComingSoon(db, cs, env.ContentRootPath);
                await SeedCinemas(db);
                await SeedReservationShowtimesAsync(db);
                await SeedComments(db);
                await SeedExampleInteractionsAsync(db);
            }
        }

        // Lookup data is topped up rather than seeded all-or-nothing, so adding a
        // new genre/language/country does not require wiping the database.
        public static async Task SeedGenre(ApplicationDbContext db)
        {
            var existing = await db.Genre.Select(x => x.Name).ToArrayAsync();

            var all = new[]
            {
                new Genre { Name = "Fantasy"},
                new Genre { Name = "Horror"},
                new Genre { Name = "Drama"},
                new Genre { Name = "Crimy"},
                new Genre { Name = "Thriller"},
                new Genre { Name = "Comedy"},
                new Genre { Name = "Kids"},
                new Genre { Name = "Action"},
                new Genre { Name = "Adventure"},
                new Genre { Name = "Animation"},
                new Genre { Name = "Biography"},
                new Genre { Name = "Documentary"},
                new Genre { Name = "Family"},
                new Genre { Name = "History"},
                new Genre { Name = "Musical"},
                new Genre { Name = "Mystery"},
                new Genre { Name = "Romance"},
                new Genre { Name = "Sci-Fi"},
                new Genre { Name = "Sport"},
                new Genre { Name = "War"},
            };

            db.Genre.AddRange(all.Where(x => !existing.Contains(x.Name)));

            await db.SaveChangesAsync();
        }

        public async static Task SeedLanguage(ApplicationDbContext db)
        {
            var existing = await db.Languages.Select(x => x.Name).ToArrayAsync();

            var all = new[]
            {
                new Language { Name = "English"},
                new Language { Name = "German"},
                new Language { Name = "French"},
                new Language { Name = "Italian"},
                new Language { Name = "Bulgarian"},
                new Language { Name = "Turkish"},
                new Language { Name = "Russish"},
                new Language { Name = "Spanish"},
                new Language { Name = "Portuguese"},
                new Language { Name = "Dutch"},
                new Language { Name = "Polish"},
                new Language { Name = "Greek"},
                new Language { Name = "Swedish"},
                new Language { Name = "Norwegian"},
                new Language { Name = "Danish"},
                new Language { Name = "Japanese"},
                new Language { Name = "Korean"},
                new Language { Name = "Chinese"},
                new Language { Name = "Hindi"},
                new Language { Name = "Arabic"},
            };

            db.Languages.AddRange(all.Where(x => !existing.Contains(x.Name)));

            await db.SaveChangesAsync();
        }

        public async static Task SeedCountry(ApplicationDbContext db)
        {
            var existing = await db.Country.Select(x => x.Name).ToArrayAsync();

            var all = new[]
            {
                new Country { Name = "USA"},
                new Country { Name = "Germany"},
                new Country { Name = "Bulgaria"},
                new Country { Name = "Turkey"},
                new Country { Name = "Spain"},
                new Country { Name = "Italy"},
                new Country { Name = "Russia"},
                new Country { Name = "France"},
                new Country { Name = "United Kingdom"},
                new Country { Name = "Canada"},
                new Country { Name = "Australia"},
                new Country { Name = "Japan"},
                new Country { Name = "South Korea"},
                new Country { Name = "China"},
                new Country { Name = "India"},
                new Country { Name = "Brazil"},
                new Country { Name = "Mexico"},
                new Country { Name = "Sweden"},
                new Country { Name = "Netherlands"},
                new Country { Name = "Poland"},
            };

            db.Country.AddRange(all.Where(x => !existing.Contains(x.Name)));

            await db.SaveChangesAsync();

        }

        public static async Task SeedUsers(IServiceProvider serviceProvider)
        {

            using var scopedServices = serviceProvider.CreateScope();

            UserManager<IdentityUser> userManager = scopedServices.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            RoleManager<IdentityRole> roleManager = scopedServices.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var db = scopedServices.ServiceProvider.GetService<ApplicationDbContext>();

            await RemoveDefaultDemoPasswordAsync(userManager, "Admin1@abv.bg");
            await RemoveDefaultDemoPasswordAsync(userManager, "User1@abv.bg");

            if (await db.Users.AnyAsync())
            {
                return;
            }

            IdentityRole roleAdmin = new()
            {
                Name = RoleNames.Admin
            };

            IdentityRole roleUser = new()
            {
                Name = RoleNames.User
            };

            await roleManager.CreateAsync(roleAdmin);
            await roleManager.CreateAsync(roleUser);

            var admin = new IdentityUser { UserName = "Admin1@abv.bg", Email = "Admin1@abv.bg", EmailConfirmed = true };
            await userManager.CreateAsync(admin);
            await userManager.AddToRoleAsync(admin, RoleNames.Admin);

            var user = new IdentityUser { UserName = "User1@abv.bg", Email = "User1@abv.bg", EmailConfirmed = true };
            await userManager.CreateAsync(user);
            await userManager.AddToRoleAsync(user, RoleNames.User);

            await db.SaveChangesAsync();
        }

        private static async Task RemoveDefaultDemoPasswordAsync(
            UserManager<IdentityUser> userManager, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null || !await userManager.CheckPasswordAsync(user, email))
            {
                return;
            }

            var result = await userManager.RemovePasswordAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Could not remove a default development demo password.");
            }
        }

        /// <summary>Create demo films or correct known legacy poster assignments.</summary>
        public static async Task SeedMovies(ApplicationDbContext db, IMovieService ms, string contentRoot)
        {
            var movies = JsonConvert.DeserializeObject<ICollection<AddMovieInputModel>>(
                await File.ReadAllTextAsync(Path.Combine(contentRoot, "movies.json")));

            if (await db.Movies.AnyAsync())
            {
                await UpdateLegacyDemoPostersAsync(db, movies);
                var existing = await db.Movies.Select(movie => movie.Title).ToListAsync();
                movies = movies.Where(movie => movie.Title.StartsWith("Demo:", StringComparison.Ordinal) &&
                    !existing.Contains(movie.Title)).ToArray();
            }

            foreach (var movie in movies)
            {
                await ms.AddMovie(movie);

            }

            await db.SaveChangesAsync();
        }

        /// <summary>Replace known incorrect poster URLs on untouched demo films.</summary>
        private static async Task UpdateLegacyDemoPostersAsync(
            ApplicationDbContext db, ICollection<AddMovieInputModel> seedMovies)
        {
            const string sharedPoster = "https://static.metacritic.com/images/products/movies/5/3cf883810a3ece9c6433ef0ababefb33-98.jpg";
            const string blackWidowPoster = "https://de.web.img2.acsta.net/c_310_420/pictures/21/07/01/12/20/0398168.jpg";
            var legacyMovies = await db.Movies.Where(movie => movie.Creator == "Admin1@abv.bg" &&
                ((movie.Title == "Oppenheimer" || movie.Title == "Everything Everywhere") && movie.ImageUrl == sharedPoster ||
                    movie.Title == "Dune" && movie.ImageUrl == blackWidowPoster)).ToListAsync();

            foreach (var movie in legacyMovies)
            {
                movie.ImageUrl = seedMovies.First(seed => seed.Title == movie.Title).Image;
            }

            if (legacyMovies.Count > 0)
            {
                await db.SaveChangesAsync();
            }
        }

        /// <summary>Seed news and add new development examples only once.</summary>
        public static async Task SeedNews(ApplicationDbContext db, INewsService ns, string contentRoot)
        {
            var news = JsonConvert.DeserializeObject<ICollection<NewsAddModel>>(await File.ReadAllTextAsync(Path.Combine(contentRoot, "news.json")));
            var existing = await db.News.Select(item => item.Title).ToListAsync();

            foreach (var item in news.Where(item => existing.Count == 0 ||
                item.Title.StartsWith("Demo:", StringComparison.Ordinal) && !existing.Contains(item.Title)))
            {
                await ns.Add(item);

            }

            await db.SaveChangesAsync();
        }

        /// <summary>Seed upcoming films and add new development examples only once.</summary>
        public static async Task SeedComingSoon(ApplicationDbContext db, IComingSoonService cs, string contentRoot)
        {
            var soon = JsonConvert.DeserializeObject<ICollection<ComingSoonAddModel>>(await File.ReadAllTextAsync(Path.Combine(contentRoot, "comingSoon.json")));
            var existing = await db.ComingSoons.Select(item => item.Title).ToListAsync();

            foreach (var item in soon.Where(item => existing.Count == 0 ||
                item.Title.StartsWith("Demo:", StringComparison.Ordinal) && !existing.Contains(item.Title)))
            {
                await cs.Add(item);

            }

            await db.SaveChangesAsync();
        }

        /// <summary>Keep a small and a one-seat example available in the development calendar.</summary>
        private static async Task SeedReservationShowtimesAsync(ApplicationDbContext db)
        {
            var owner = await db.Users.FirstOrDefaultAsync(user => user.Email == "Admin1@abv.bg");
            if (owner is null)
            {
                return;
            }

            var cinema = await db.Cinemas.FirstOrDefaultAsync(item => item.Name == "Demo Cinema" && item.UserId == owner.Id)
                ?? new Cinema { Name = "Demo Cinema", UserId = owner.Id };
            var hall = await db.CinemaHalls.FirstOrDefaultAsync(item => item.Name == "Demo Hall")
                ?? new CinemaHall { Name = "Demo Hall" };
            var today = DateTime.Today;
            foreach (var (title, time, rows, columns) in new[]
            {
                ("Demo: Last available seat", "14:00", 1, 1),
                ("Demo: The Night Projection", "18:30", 4, 6),
                ("Demo: The Last Signal", "20:30", 6, 8)
            })
            {
                if (!await db.CinemaPojections.AnyAsync(item => item.CinemaId == cinema.Id &&
                    item.MovieName == title && item.ProjectionTime >= today))
                {
                    db.CinemaPojections.Add(new CinemaPojection
                    {
                        Cinema = cinema, CinemaHall = hall, MovieName = title, Time = time,
                        ProjectionTime = today.AddDays(1), SeatRows = rows, SeatsPerRow = columns
                    });
                }
            }

            await db.SaveChangesAsync();
        }

        /// <summary>Add sample comments, ratings, and likes to fictional development films.</summary>
        private static async Task SeedExampleInteractionsAsync(ApplicationDbContext db)
        {
            var examples = await db.Movies.Where(item => item.Creator == "Admin1@abv.bg" &&
                item.Title.StartsWith("Demo:")).ToListAsync();
            var viewer = await db.Users.FirstOrDefaultAsync(user => user.Email == "User1@abv.bg");
            if (viewer is null)
            {
                return;
            }

            const string example = "Demo comment: I would watch this at an evening screening.";
            foreach (var movie in examples)
            {
                if (!await db.MovieComments.AnyAsync(item => item.MovieId == movie.Id && item.Comment.Description == example))
                {
                    db.MovieComments.Add(new MovieComment
                    {
                        MovieId = movie.Id,
                        Comment = new Comment { Description = example, UserEmail = viewer.Email }
                    });
                }

                if (movie.RatingCount == 0)
                {
                    movie.Rating = 9;
                    movie.RatingCount = 2;
                }

                if (!await db.UserMovies.AnyAsync(item => item.MovieId == movie.Id && item.UserId == viewer.Id))
                {
                    db.UserMovies.Add(new UserMovie { MovieId = movie.Id, UserId = viewer.Id });
                    movie.Likes++;
                }
            }

            await db.SaveChangesAsync();
        }

        public static async Task SeedCinemas(ApplicationDbContext db)
        {
            if (db.Cinemas.Any())
            {
                return;
            }

            var owner = await db.Users.FirstOrDefaultAsync();
            if (owner == null)
            {
                return;
            }

            var cinemas = new[]
            {
                new Cinema { Name = "Cinema City", UserId = owner.Id },
                new Cinema { Name = "Arena Mall", UserId = owner.Id },
                new Cinema { Name = "Grand Palace", UserId = owner.Id },
            };

            var halls = new[]
            {
                new CinemaHall { Name = "Hall 1" },
                new CinemaHall { Name = "Hall 2" },
                new CinemaHall { Name = "Hall 3" },
                new CinemaHall { Name = "IMAX" },
                new CinemaHall { Name = "VIP Lounge" },
            };

            db.Cinemas.AddRange(cinemas);
            db.CinemaHalls.AddRange(halls);
            await db.SaveChangesAsync();

            // Projections are spread over the coming week so the "In Cinema"
            // page (which only shows the next 7 days) is never empty.
            var movieTitles = await db.Movies.Select(x => x.Title).Take(20).ToArrayAsync();
            if (movieTitles.Length == 0)
            {
                return;
            }

            var startTimes = new[] { "14:00", "16:30", "18:00", "20:15", "22:30" };
            var projections = new List<CinemaPojection>();

            for (int i = 0; i < 20; i++)
            {
                projections.Add(new CinemaPojection
                {
                    MovieName = movieTitles[i % movieTitles.Length],
                    ProjectionTime = DateTime.UtcNow.Date.AddDays(i % 7),
                    Time = startTimes[i % startTimes.Length],
                    CinemaId = cinemas[i % cinemas.Length].Id,
                    CinemaHallId = halls[i % halls.Length].Id,
                });
            }

            db.CinemaPojections.AddRange(projections);
            await db.SaveChangesAsync();
        }

        public static async Task SeedComments(ApplicationDbContext db)
        {
            if (db.Comments.Any())
            {
                return;
            }

            var movieIds = await db.Movies.Select(x => x.Id).Take(20).ToArrayAsync();
            if (movieIds.Length == 0)
            {
                return;
            }

            var texts = new[]
            {
                "Absolutely loved it, the ending caught me completely off guard.",
                "Great cast, but the second half drags a bit.",
                "The soundtrack alone is worth the ticket.",
                "Visually stunning - watch this one on the biggest screen you can find.",
                "Solid popcorn movie. Nothing more, nothing less.",
                "I went in with low expectations and walked out impressed.",
                "The pacing is off, but the performances carry it.",
                "One of the best things I have seen this year.",
                "Nice idea, weak execution. Still worth a watch.",
                "My whole family enjoyed this one.",
                "The trailer spoiled way too much, but I still had fun.",
                "Rewatched it yesterday and noticed so many small details.",
                "A bit too long for my taste, but the finale delivers.",
                "The cinematography is the real star here.",
                "Not for everyone, but I could not look away.",
                "Funnier than I expected. The dialogue is sharp.",
                "Decent, though it borrows heavily from better films.",
                "Perfect movie for a rainy evening.",
                "The lead performance deserves an award.",
                "Underrated. I have no idea why the reviews were so harsh.",
            };

            var authors = new[] { "Admin1@abv.bg", "User1@abv.bg" };

            for (int i = 0; i < texts.Length; i++)
            {
                var comment = new Comment
                {
                    Description = texts[i],
                    UserEmail = authors[i % authors.Length],
                    CreatedOn = DateTime.UtcNow.AddDays(-i),
                };

                db.Comments.Add(comment);
                await db.SaveChangesAsync();

                db.MovieComments.Add(new MovieComment
                {
                    MovieId = movieIds[i % movieIds.Length],
                    CommentId = comment.Id,
                });
            }

            await db.SaveChangesAsync();
        }
    }
}

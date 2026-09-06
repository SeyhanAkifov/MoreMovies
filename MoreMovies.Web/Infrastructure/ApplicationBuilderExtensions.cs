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
        // Called (and blocked on) from Startup.Configure so it also runs under
        // WebApplicationFactory-based integration tests, which never execute Program.Main.
        public static async Task PrepareDatabaseAsync(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();
            var services = scopedServices.ServiceProvider;

            var db = services.GetRequiredService<ApplicationDbContext>();
            var ms = services.GetRequiredService<IMovieService>();
            var ns = services.GetRequiredService<INewsService>();
            var cs = services.GetRequiredService<IComingSoonService>();

            db.Database.Migrate();

            await SeedGenre(db);
            await SeedLanguage(db);
            await SeedCountry(db);

            if (env.IsDevelopment())
            {
                await SeedUsers(services);
                await SeedMovies(db, ms);
                await SeedNews(db, ns);
                await SeedComingSoon(db, cs);
                await SeedCinemas(db);
                await SeedComments(db);
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

            if (db.Users.Any())
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
            await userManager.CreateAsync(admin, "Admin1@abv.bg");
            await userManager.AddToRoleAsync(admin, RoleNames.Admin);

            var user = new IdentityUser { UserName = "User1@abv.bg", Email = "User1@abv.bg", EmailConfirmed = true };
            await userManager.CreateAsync(user, "User1@abv.bg");
            await userManager.AddToRoleAsync(user, RoleNames.User);

            await db.SaveChangesAsync();
        }

        public static async Task SeedMovies(ApplicationDbContext db, IMovieService ms)
        {
            if (db.Movies.Any())
            {
                return;
            }

            var movies = JsonConvert.DeserializeObject<ICollection<AddMovieInputModel>>(File.ReadAllText("movies.json"));

            foreach (var movie in movies)
            {
                await ms.AddMovie(movie);

            }

            await db.SaveChangesAsync();
        }

        public static async Task SeedNews(ApplicationDbContext db, INewsService ns)
        {
            if (db.News.Any())
            {
                return;
            }

            var news = JsonConvert.DeserializeObject<ICollection<NewsAddModel>>(File.ReadAllText("news.json"));

            foreach (var item in news)
            {
                await ns.Add(item);

            }

            await db.SaveChangesAsync();
        }

        public static async Task SeedComingSoon(ApplicationDbContext db, IComingSoonService cs)
        {
            if (db.ComingSoons.Any())
            {
                return;
            }

            var soon = JsonConvert.DeserializeObject<ICollection<ComingSoonAddModel>>(File.ReadAllText("comingSoon.json"));

            foreach (var item in soon)
            {
                await cs.Add(item);

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

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
            }
        }

        public static async Task SeedGenre(ApplicationDbContext db)
        {
            if (db.Genre.Any())
                return;

            db.Genre.AddRange(new[]
            {
                new Genre { Name = "Fantasy"},
                new Genre { Name = "Horror"},
                new Genre { Name = "Drama"},
                new Genre { Name = "Crimy"},
                new Genre { Name = "Thriller"},
                new Genre { Name = "Comedy"},
                new Genre { Name = "Kids"},
                new Genre { Name = "Action"},
            });

            await db.SaveChangesAsync();
        }

        public async static Task SeedLanguage(ApplicationDbContext db)
        {
            if (db.Languages.Any())
                return;

            db.Languages.AddRange(new[]
            {
                new Language { Name = "English"},
                new Language { Name = "German"},
                new Language { Name = "French"},
                new Language { Name = "Italian"},
                new Language { Name = "Bulgarian"},
                new Language { Name = "Turkish"},
                new Language { Name = "Russish"},
                new Language { Name = "Spanish"},
            });

            await db.SaveChangesAsync();
        }

        public async static Task SeedCountry(ApplicationDbContext db)
        {
            if (db.Country.Any())
            {
                return;
            }

            db.Country.AddRange(new[]
            {
                new Country { Name = "USA"},
                new Country { Name = "Germany"},
                new Country { Name = "Bulgaria"},
                new Country { Name = "Turkey"},
                new Country { Name = "Spain"},
                new Country { Name = "Italy"},
                new Country { Name = "Russia"},
                new Country { Name = "France"},
            });

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
    }
}

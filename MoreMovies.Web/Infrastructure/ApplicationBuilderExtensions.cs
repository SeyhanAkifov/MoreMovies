using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoreMovies.Web.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {



        public static IApplicationBuilder PrepareDatabase(this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();


            var db = scopedServices.ServiceProvider.GetService<ApplicationDbContext>();
            var ms = scopedServices.ServiceProvider.GetService<IMovieService>();
            var ns = scopedServices.ServiceProvider.GetService<INewsService>();
            var cs = scopedServices.ServiceProvider.GetService<IComingSoonService>();


            db.Database.Migrate();

            SeedGenre(db);
            SeedLanguage(db);
            SeedCountry(db);
            SeedUsers(app.ApplicationServices);
            SeedMovies(db, ms);
            SeedNews(db, ns);
            SeedComingSoon(db, cs);

            return app;
        }

        public async static void SeedGenre(ApplicationDbContext db)
        {

            if (db.Genre.Any())
            {
                return;
            }

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

        public async static void SeedLanguage(ApplicationDbContext db)
        {
            if (db.Languages.Any())
            {
                return;
            }

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

        public async static void SeedCountry(ApplicationDbContext db)
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

        public static async void SeedUsers(IServiceProvider serviceProvider)
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
                Name = "Admin"
            };

            IdentityRole roleUser = new()
            {
                Name = "User"
            };

            await roleManager.CreateAsync(roleAdmin);
            await roleManager.CreateAsync(roleUser);

            var admin = new IdentityUser { UserName = "Admin1@abv.bg", Email = "Admin1@abv.bg", EmailConfirmed = true };
            await userManager.CreateAsync(admin, "Admin1@abv.bg");
            await userManager.AddToRoleAsync(admin, "Admin");

            var user = new IdentityUser { UserName = "User1@abv.bg", Email = "User1@abv.bg", EmailConfirmed = true };
            await userManager.CreateAsync(user, "User1@abv.bg");
            await userManager.AddToRoleAsync(user, "User");

            await db.SaveChangesAsync();
        }

        public static async void SeedMovies(ApplicationDbContext db, IMovieService ms)
        {
            if (db.Movies.Any())
            {
                return;
            }

            var movies = JsonConvert.DeserializeObject<ICollection<AddMovieInputModel>>(File.ReadAllText("movies.json"));

            foreach (var movie in movies)
            {
                ms.AddMovie(movie);

            }

            await db.SaveChangesAsync();
        }

        public static async void SeedNews(ApplicationDbContext db, INewsService ns)
        {
            if (db.News.Any())
            {
                return;
            }

            var news = JsonConvert.DeserializeObject<ICollection<NewsAddModel>>(File.ReadAllText("news.json"));

            foreach (var item in news)
            {
                ns.Add(item);

            }

            await db.SaveChangesAsync();
        }

        public static async void SeedComingSoon(ApplicationDbContext db, IComingSoonService cs)
        {
            if (db.ComingSoons.Any())
            {
                return;
            }

            var soon = JsonConvert.DeserializeObject<ICollection<ComingSoonAddModel>>(File.ReadAllText("comingSoon.json"));

            foreach (var item in soon)
            {
                cs.Add(item);

            }

            await db.SaveChangesAsync();
        }
    }
}

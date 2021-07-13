using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoreMovies.Data;
using MoreMovies.Models;
using System;
using System.Linq;

namespace MoreMovies.Web.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        

        
        public static IApplicationBuilder PrepareDatabase(this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();

            var db = scopedServices.ServiceProvider.GetService<ApplicationDbContext>();
            
            db.Database.Migrate();

            SeedGenre(db);
            SeedLanguage(db);
            SeedCountry(db);
            SeedUsers(app.ApplicationServices);

            return app;
        }

        public static void SeedGenre(ApplicationDbContext db)
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

            db.SaveChanges();

        }

        public static void SeedLanguage(ApplicationDbContext db)
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

            db.SaveChanges();

        }

        public static void SeedCountry(ApplicationDbContext db)
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

            db.SaveChanges();

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

            IdentityRole roleAdmin = new IdentityRole
            {
                Name = "Admin"
            };

            IdentityRole roleUser = new IdentityRole
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
    }
}

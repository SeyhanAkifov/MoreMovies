using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MoreMovies.Data;
using MoreMovies.Services;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Hubs;
using MoreMovies.Web.Infrastructure;
using MoreMovies.Web.MappingConfiguration;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.MaxLengthForKeys = 0;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

services.AddControllersWithViews(options
    => options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>());

services.AddAutoMapper(typeof(ApplicationProfile));

services.AddSignalR();

services.AddScoped<IMovieService, MovieService>();
services.AddScoped<ILanguageService, LanguageService>();
services.AddScoped<IGenreService, GenreService>();
services.AddScoped<IActorService, ActorService>();
services.AddScoped<ICountryService, CountryService>();
services.AddScoped<ICommentService, CommentService>();
services.AddScoped<INewsService, NewsService>();
services.AddScoped<IComingSoonService, ComingSoonService>();
services.AddScoped<ICinemaService, CinemaService>();
services.AddScoped<IUserService, UserService>();
services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("default", "{controller=Home}/{action=Index}");
app.MapRazorPages();
app.MapHub<MovieHub>("/moviehub");

await app.PrepareDatabaseAsync(app.Environment);
await app.RunAsync();

public partial class Program
{
}

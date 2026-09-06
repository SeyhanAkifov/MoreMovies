using AutoMapper;
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

namespace MoreMovies.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        { 
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();



            services.AddControllersWithViews(options
                => options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>());

            services.AddAutoMapper(typeof(ApplicationProfile));

            services.AddMvc();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Blocking (not fire-and-forget) so a migration/seed failure aborts startup
            // instead of being silently swallowed, and so this also runs under
            // WebApplicationFactory-based integration tests (which never call Program.Main).
            app.PrepareDatabaseAsync(env).GetAwaiter().GetResult();

            if (env.IsDevelopment())
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
                endpoints.MapRazorPages();
                endpoints.MapHub<MovieHub>("/moviehub");
            });
        }
    }
}

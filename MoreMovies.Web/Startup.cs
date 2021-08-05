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
using SocialNetworkCustom.Web.MappingConfiguration;

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

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ApplicationProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc();

            services.AddSignalR();

            services.AddTransient<IMovieService, MovieService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IGenreService, GenreService>();
            services.AddTransient<IActorService, ActorService>();
            services.AddTransient<ICountryService, CountryService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<INewsService, NewsService>();
            services.AddTransient<IComingSoonService, ComingSoonService>();
            services.AddTransient<ICinemaService, CinemaService>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<UserManager<IdentityUser>>();
            services.AddScoped<IdentityUser>();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.PrepareDatabase();

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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<MovieHub>("/moviehub");
            });
        }
    }
}

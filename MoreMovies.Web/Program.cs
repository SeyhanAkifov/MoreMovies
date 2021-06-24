using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MoreMovies.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var db = new ApplicationDbContext();
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();
            //db.Database.Migrate();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

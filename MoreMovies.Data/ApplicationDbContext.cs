using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoreMovies.Models;

namespace MoreMovies.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("");

              

            }
        }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<MovieComment> MovieComments { get; set; }
        public DbSet<MovieCountry> MovieCountries { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<MovieLanguage> MovieLanguages { get; set; }
        public DbSet<UserMovie> UserMovies { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<ComingSoon> ComingSoons { get; set; }
        public DbSet<CinemaHall> CinemaHalls { get; set; }
        public DbSet<CinemaPojection> CinemaPojections { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<ScreeningSeatHold> ScreeningSeatHolds { get; set; }

        /// <summary>Configure the database model and relationship keys.</summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ScreeningSeatHold>(hold =>
            {
                hold.HasKey(seat => new { seat.ScreeningId, seat.SeatNumber });
                hold.Property(seat => seat.UserId).IsRequired().HasMaxLength(450);
                hold.HasOne<CinemaPojection>().WithMany()
                    .HasForeignKey(seat => seat.ScreeningId);
            });

            modelBuilder.Entity<Movie>()
                .Property(movie => movie.Budget)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MovieActor>()
                .HasKey(x => new { x.ActorId, x.MovieId });

            modelBuilder.Entity<MovieLanguage>()
                .HasKey(x => new { x.LanguageId, x.MovieId });

            modelBuilder.Entity<MovieGenre>()
                .HasKey(x => new { x.GenreId, x.MovieId });

            modelBuilder.Entity<MovieCountry>()
                .HasKey(x => new { x.CountryId, x.MovieId });

            modelBuilder.Entity<MovieComment>()
                .HasKey(x => new { x.CommentId, x.MovieId });

            modelBuilder.Entity<UserMovie>()
                .HasKey(x => new { x.UserId, x.MovieId });
        }
    }
}

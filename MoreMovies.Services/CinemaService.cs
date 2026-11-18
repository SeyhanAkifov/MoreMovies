using Microsoft.EntityFrameworkCore;
using MoreMovies.Data;
using MoreMovies.Models;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services
{
    public class CinemaService : ICinemaService
    {

        private readonly ApplicationDbContext db;
        private readonly IUserService userService;
        private readonly TimeProvider clock;


        /// <summary>Initialize cinema services and the cancellation clock.</summary>
        public CinemaService(ApplicationDbContext db, IUserService userService, TimeProvider clock)
        {
            this.db = db;
            this.userService = userService;
            this.clock = clock;
        }

        public List<string> GetCinema(string userName)
        {
            return this.db.Cinemas.Where(x => x.User.Email == userName).Select(x => x.Name).ToList();
        }

        public async Task AddCinema(string cinemaName, string userName)
        {
            var user = await this.userService.GetUser(userName);


            await this.userService.Become(user.Id);
            var cinema = new Cinema
            {
                Name = cinemaName,
                User = user,
            };

            this.db.Cinemas.Add(cinema);

            await db.SaveChangesAsync();
        }

        public async Task<bool> Add(CinemaProjectionInputDto model, string userId)
        {
            var cinema = await db.Cinemas.FirstOrDefaultAsync(x =>
                x.Name == model.CinemaName && x.UserId == userId);
            if (cinema == null)
            {
                return false;
            }

            var hall = await db.CinemaHalls.FirstOrDefaultAsync(x => x.Name == model.CinemaHallName)
                ?? new CinemaHall { Name = model.CinemaHallName };

            this.db.CinemaPojections.Add(new CinemaPojection
            {
                Time = model.Time,
                MovieName = model.MovieName,
                ProjectionTime = model.ProjectionTime,
                CinemaHall = hall,
                CinemaId = cinema.Id,
                SeatRows = model.SeatRows,
                SeatsPerRow = model.SeatsPerRow
            });

            await this.db.SaveChangesAsync();
            return true;
        }

        /// <summary>Cancel an owned screening and clear only its unconfirmed holds.</summary>
        public async Task<bool> CancelAsync(int id, string userId)
        {
            await using var transaction = await db.Database.BeginTransactionAsync();
            var screening = await db.LockScreeningAsync(id);
            if (screening is null || !await db.Cinemas.AnyAsync(cinema =>
                    cinema.Id == screening.CinemaId && cinema.UserId == userId))
            {
                return false;
            }

            if (screening.CancelledAt.HasValue)
            {
                return true;
            }

            var cancelledAt = clock.GetUtcNow();
            await db.CinemaPojections.Where(projection => projection.Id == id)
                .ExecuteUpdateAsync(update => update.SetProperty(projection => projection.CancelledAt, cancelledAt));
            await db.ScreeningSeatHolds
                .Where(hold => hold.ScreeningId == id && hold.ConfirmedAt == null)
                .ExecuteDeleteAsync();
            await transaction.CommitAsync();
            return true;
        }

        /// <summary>List distinct cinema names in alphabetical order.</summary>
        public List<string> GetCinemaNames()
        {
            return db.Cinemas.Select(x => x.Name).Distinct().OrderBy(name => name).ToList();
        }

        

        /// <summary>List showtimes and indicate which ones the viewer owns.</summary>
        public ICollection<CinemaProjectionOutputDto> GetAll(string cinemaName, string userId = null)
        {

            if (string.IsNullOrEmpty(cinemaName))
            {
                cinemaName = this.db.Cinemas.FirstOrDefault()?.Name;
            }
            return this.db.CinemaPojections
                .Where(x => x.Cinema.Name == cinemaName)
                .OrderBy(x => x.ProjectionTime)
                .ThenBy(x => x.Time)
                .Select(x => new CinemaProjectionOutputDto
            {
                Id = x.Id,
                CanManage = userId != null && x.Cinema.UserId == userId,
                Cancelled = x.CancelledAt.HasValue,
                HasSeatPlan = x.SeatRows > 0 && x.SeatsPerRow > 0,
                MovieName = x.MovieName,
                ProjectionTime = x.ProjectionTime,
                CinemaHall = x.CinemaHall,
                Time = x.Time
                
            }) .ToList();
        }
    }
}

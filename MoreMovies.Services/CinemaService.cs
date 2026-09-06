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


        public CinemaService(ApplicationDbContext db, IUserService userService)
        {
            this.db = db;
            this.userService = userService;
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

        public async Task Add(CinemaProjectionInputDto model)
        {
            var hall = await this.db.CinemaHalls.FirstOrDefaultAsync(x => x.Name == model.CinemaHallName);
            if (hall == null)
            {
                hall = new CinemaHall
                {
                    Name = model.CinemaHallName,
                };

                this.db.CinemaHalls.Add(hall);

                await this.db.SaveChangesAsync();
            }

            var cinema = await this.db.Cinemas.FirstOrDefaultAsync(x => x.Name == model.CinemaName);

            this.db.CinemaPojections.Add(new CinemaPojection
            {
                Time = model.Time,
                MovieName = model.MovieName,
                ProjectionTime = model.ProjectionTime,
                CinemaHallId = hall.Id,
                CinemaHall = hall,
                CinemaId = cinema.Id
            });

            await this.db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var cinemaProjection = await this.db.CinemaPojections.FindAsync(id);

            this.db.CinemaPojections.Remove(cinemaProjection);

            await this.db.SaveChangesAsync();
        }

        public List<string> GetCinemaNames()
        {
            return this.db.Cinemas.Select(x => x.Name).ToList();
        }

        

        public ICollection<CinemaProjectionOutputDto> GetAll(string cinemaName)
        {

            if (string.IsNullOrEmpty(cinemaName))
            {
                cinemaName = this.db.Cinemas.FirstOrDefault()?.Name;
            }
            return this.db.CinemaPojections
                .Where(x => x.Cinema.Name == cinemaName)
                .Select(x => new CinemaProjectionOutputDto
            {
                MovieName = x.MovieName,
                ProjectionTime = x.ProjectionTime,
                CinemaHall = x.CinemaHall,
                Time = x.Time
                
            }) .ToList();
        }
    }
}

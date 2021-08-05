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

        public CinemaService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void Add(CinemaProjectionInputDto model)
        {
            

            var hall = this.db.CinemaHalls.FirstOrDefault(x => x.Name == model.CinemaHallName);
            if (hall == null)
            {
                this.db.CinemaHalls.Add(new CinemaHall
                {
                    Name = model.CinemaHallName,
                });

                this.db.SaveChanges();

                hall = this.db.CinemaHalls.FirstOrDefault(x => x.Name == model.CinemaHallName);
            }

          

            this.db.CinemaPojections.Add(new CinemaPojection
            {
                Time = model.Time,
                MovieName = model.MovieName,
                ProjectionTime = model.ProjectionTime,
                CinemaHallId = hall.Id,
                CinemaHall = hall,
            });

            this.db.SaveChanges();
        }

        public void Delete(int id)
        {
            var cinemaProjection = this.db.CinemaPojections.Find(id);

            this.db.CinemaPojections.Remove(cinemaProjection);
        }

        public ICollection<CinemaProjectionOutputDto> GetAll()
        {
            return this.db.CinemaPojections.Select(x => new CinemaProjectionOutputDto
            {
                MovieName = x.MovieName,
                ProjectionTime = x.ProjectionTime,
                CinemaHall = x.CinemaHall,
                Time = x.Time
                
            }).ToList();
        }
    }
}

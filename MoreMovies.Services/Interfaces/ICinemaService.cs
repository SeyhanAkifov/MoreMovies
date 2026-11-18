using MoreMovies.Models;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface ICinemaService
    {
        public Task<bool> Add(CinemaProjectionInputDto model, string userId);

        /// <summary>Cancel an owned screening and retain its confirmed bookings.</summary>
        public Task<bool> CancelAsync(int id, string userId);

        /// <summary>List showtimes and indicate which ones the viewer owns.</summary>
        public ICollection<CinemaProjectionOutputDto> GetAll(string cinemaName, string userId = null);

        public Task AddCinema(string cinemaName, string userName);

        public List<string> GetCinema(string cinemaName);

        public List<string> GetCinemaNames();
    }
}

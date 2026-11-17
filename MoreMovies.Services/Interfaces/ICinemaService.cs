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

        public Task<bool> Delete(int id, string userId);

        public ICollection<CinemaProjectionOutputDto> GetAll(string cinemaName);

        public Task AddCinema(string cinemaName, string userName);

        public List<string> GetCinema(string cinemaName);

        public List<string> GetCinemaNames();
    }
}

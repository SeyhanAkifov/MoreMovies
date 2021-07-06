using MoreMovies.Models;
using MoreMovies.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface IComingSoonService
    {
        public Task Add(ComingSoonAddModel model);
        public Task Delete(int id);

        public Task<ComingSoon> GetWithId(int id);

        public Task<ICollection<ComingSoon>> GetAll();

        public Task<ICollection<ComingSoon>> GetForHomePage();
    }
}

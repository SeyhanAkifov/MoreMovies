using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface IComingSoonService
    {
        public Task Add(ComingSoonAddModel model);

        public Task Delete(int id);

        public Task<ComingSoonOutputDto> GetWithId(int id);

        public Task<ICollection<ComingSoonOutputDto>> GetAll();

        public Task<ICollection<ComingSoonOutputDto>> GetForHomePage();
    }
}

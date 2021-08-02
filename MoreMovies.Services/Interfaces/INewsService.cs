using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface INewsService
    {
        public Task Add(NewsAddModel model);

        public Task Delete(int id);

        public Task<NewsOutputDto> GetNewsWithId(int id);

        public Task<ICollection<NewsOutputDto>> GetAllNews();

        public Task<ICollection<NewsOutputDto>> GetNewsForHomePage();
    }
}

using MoreMovies.Models;
using MoreMovies.Services.Dto.Input;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface INewsService
    {
        public Task Add(NewsAddModel model);
        public Task Delete(int id);

        public Task<News> GetNewsWithId(int id);

        public Task<ICollection<News>> GetAllNews();

        public Task<ICollection<News>> GetNewsForHomePage();
    }
}

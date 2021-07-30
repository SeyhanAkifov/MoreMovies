using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Models;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[action]/{id}")]
    public class DetailsApiController : ControllerBase
    {
        private readonly IMovieService movieService;
        private readonly IMapper mapper;

        public DetailsApiController(IMovieService movieService, IMapper mapper)
        {
            this.movieService = movieService;
            this.mapper = mapper;
        }


        public async Task<MovieViewModel> GetDetails(int id)
        {
            var details = await movieService.GetDetails(id);
            var result = mapper.Map<Movie, MovieViewModel>(details);

            return result;
        }
    }
}

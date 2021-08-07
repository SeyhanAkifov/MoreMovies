using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models.Movie;
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

        public async Task<IActionResult> GetDetails(int id)
        {
            if(id < 1)
            {
                return BadRequest($"Invalid request for id {id}");
            }
            var details = await movieService.GetDetails(id);
            var result = mapper.Map<DetailsOutputDto, MovieDetailsApiModel>(details);

            return Ok(result);
        }
    }
}

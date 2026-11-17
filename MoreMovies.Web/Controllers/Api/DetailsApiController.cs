using MoreMovies.Web.MappingConfiguration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Services.Interfaces;
using MoreMovies.Web.Models.Movie;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/[action]/{id}")]
    public class DetailsApiController : ControllerBase
    {
        private readonly IMovieService movieService;

        public DetailsApiController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        public async Task<IActionResult> GetDetails(int id)
        {
            if(id < 1)
            {
                return BadRequest($"Invalid request for id {id}");
            }
            var details = await movieService.GetDetails(id);
            var result = details.ToApiModel();

            return Ok(result);
        }
    }
}

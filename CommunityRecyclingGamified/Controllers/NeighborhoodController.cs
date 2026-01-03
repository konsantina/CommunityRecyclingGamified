using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityRecyclingGamified.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NeighborhoodController: ControllerBase
    {
        private readonly INeighborhoodRepository _neighborhoodRepository;
        public NeighborhoodController(INeighborhoodRepository neighborhoodRepository)
        {
         _neighborhoodRepository = neighborhoodRepository;   
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Neighborhood>>> GetAllNeighborhoods() 
        {
          var neighborhoods = await _neighborhoodRepository.GetAllAsync();

            if (neighborhoods == null)
            {
                return NotFound();
            }

            return Ok(neighborhoods);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Neighborhood), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Neighborhood>> GetNeighborhoodById(int id)
        {
            var result = await _neighborhoodRepository.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}

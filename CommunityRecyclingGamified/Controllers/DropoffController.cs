using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityRecyclingGamified.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DropoffController : ControllerBase
    {
        public readonly IDropoffRepository _dropoffRepository;

        public DropoffController(IDropoffRepository dropoffRepository)
        {
            _dropoffRepository = dropoffRepository;
        }

        //Post: api/Dropoff
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Dropoff>> CreateDropoff([FromBody] DropoffDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dropoff = new Dropoff
            {
                UserId = dto.UserId,
                MaterialId = dto.MaterialId,
                NeighborhoodId = dto.NeighborhoodId,
                Quantity = dto.Quantity,
                Unit = dto.Unit,
                Location = dto.Location,
                Status = Enums.DropoffStatus.Recorded,
                PointsAwarded = 0,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _dropoffRepository.AddAsync(dropoff);

            if (!saved)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not save dropoff");

            // 201 Created + Location header
            return CreatedAtAction(nameof(GetById), new { id = dropoff.Id }, dropoff);
        }
        // GET: api/Dropoff/{id}
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Dropoff>> GetById(int id)
        {
            var dropoff = await _dropoffRepository.GetByIdAsync(id);

            if (dropoff == null)
                return NotFound();

            return Ok(dropoff);
        }
        // GET: api/Dropoff/pending
        [AllowAnonymous]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Dropoff>>> GetPending()
        {
            var dropoffs = await _dropoffRepository.GetPendingAsync();
            return Ok(dropoffs);
        }


        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] DropoffRejectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = await _dropoffRepository.RejectAsync(id, dto.VerifierUserId);

            if (!ok)
                return BadRequest("Cannot reject dropoff (not found / not recorded).");

            return NoContent();
        }


        [Authorize]
        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<DropoffListItemDto>>> GetByUser(int userId)
        {
            var items = await _dropoffRepository.GetByUserAsync(userId);
            return Ok(items);
        }

    }
}


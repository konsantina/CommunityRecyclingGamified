using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityRecyclingGamified.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DropoffController : ControllerBase
    {
        private readonly IDropoffRepository _dropoffRepository;

        public DropoffController(IDropoffRepository dropoffRepository)
        {
            _dropoffRepository = dropoffRepository;
        }

        private int GetUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(raw))
                throw new UnauthorizedAccessException("Missing user id claim.");
            return int.Parse(raw);
        }

        // POST: api/Dropoff
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Dropoff>> CreateDropoff([FromBody] DropoffDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();

            var dropoff = new Dropoff
            {
                UserId = userId,                 // ✅ από token
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not save dropoff");

            return CreatedAtAction(nameof(GetById), new { id = dropoff.Id }, dropoff);
        }

        // GET: api/Dropoff/{id}
        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Dropoff>> GetById(int id)
        {
            var dropoff = await _dropoffRepository.GetByIdAsync(id);
            if (dropoff == null) return NotFound();

            var userId = GetUserId();
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Moderator");

            if (!isAdmin && dropoff.UserId != userId)
                return Forbid();

            return Ok(dropoff);
        }


        // GET: api/Dropoff/pending
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Dropoff>>> GetPending()
        {
            var dropoffs = await _dropoffRepository.GetPendingAsync();
            return Ok(dropoffs);
        }

        // POST: api/Dropoff/{id}/verify
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("{id:int}/verify")]
        public async Task<IActionResult> Verify(int id)
        {
            var verifierUserId = GetUserId();

            var ok = await _dropoffRepository.VerifyAsync(id, verifierUserId);

            if (!ok)
                return BadRequest("Cannot verify dropoff (not found / not recorded / inactive material).");

            return NoContent();
        }

        // POST: api/Dropoff/{id}/reject
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var verifierUserId = int.Parse(raw!);

            var ok = await _dropoffRepository.RejectAsync(id, verifierUserId);

            if (!ok)
                return BadRequest("Cannot reject dropoff (not found / not recorded).");

            return NoContent();
        }

        // GET: api/Dropoff/my
        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<DropoffListItemDto>>> GetMy()
        {
            var userId = GetUserId();
            var items = await _dropoffRepository.GetByUserAsync(userId);
            return Ok(items);
        }

        // GET: api/Dropoff/user/{userId}
        [Authorize(Roles = "Admin,Moderator,User")]
        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<DropoffListItemDto>>> GetByUser(int userId)
        {
            var items = await _dropoffRepository.GetByUserAsync(userId);
            return Ok(items);
        }

        [Authorize(Roles = "User")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DropoffUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var ok = await _dropoffRepository.UpdateByOwnerAsync(id, userId, dto);

            if (!ok) return BadRequest("Cannot update dropoff (not found / not owner / not editable status).");

            return NoContent();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var ok = await _dropoffRepository.DeleteByOwnerAsync(id, userId);

            if (!ok) return BadRequest("Cannot delete dropoff (not found / not owner / not deletable status).");

            return NoContent();
        }

    }
}

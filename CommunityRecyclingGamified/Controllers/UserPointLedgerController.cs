
using global::CommunityRecyclingGamified.Dto;
using global::CommunityRecyclingGamified.Models;
using global::CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommunityRecyclingGamified.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPointLedgerController : ControllerBase
    {
        private readonly IUserPointLedgerRepository _ledgerRepository;

        public UserPointLedgerController(IUserPointLedgerRepository ledgerRepository)
        {
            _ledgerRepository = ledgerRepository;
        }

        // GET: api/UserPointLedger/user/5
        [HttpGet("user/{userId:int}")]
        public async Task<ActionResult<IEnumerable<UserPointLedger>>> GetByUser(int userId)
        {
            var entries = await _ledgerRepository.GetByUserAsync(userId);
            return Ok(entries);
        }

        // POST: api/UserPointLedger  (optional, για manual/ admin adjustments)
        [HttpPost]
        public async Task<ActionResult<UserPointLedger>> AddEntry([FromBody] UserPointLedgerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entry = new UserPointLedger
            {
                UserId = dto.UserId,
                Amount = dto.Amount,
                Reason = dto.Reason,
                RefEntityType = dto.RefEntityType,
                RefEntityId = dto.RefEntityId,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _ledgerRepository.AddAsync(entry);

            if (!saved)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not save ledger entry");

            return CreatedAtAction(nameof(GetByUser),
                new { userId = entry.UserId },
                entry);
        }
    }
}

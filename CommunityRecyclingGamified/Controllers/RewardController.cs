using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityRecyclingGamified.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RewardController : ControllerBase
    {
        private readonly IRewardRepository _rewardRepository;

        public RewardController(IRewardRepository rewardRepository)
        {
            _rewardRepository = rewardRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reward>>> GetAllRewards()
        {
            var rewards = await _rewardRepository.GetAllActiveAsync();
            return Ok(rewards);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Reward>> GetRewardsById(int id)
        {
            var reward = await _rewardRepository.GetByIdAsync(id);
            if (reward == null) return NotFound();
            return Ok(reward);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RewardCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.ValidFrom.HasValue && dto.ValidTo.HasValue && dto.ValidFrom > dto.ValidTo)
                return BadRequest("ValidFrom cannot be after ValidTo");

            var reward = new Reward
            {
                Title = dto.Title.Trim(),
                Description = (dto.Description ?? "").Trim(),
                CostPoints = dto.CostPoints,
                Stock = dto.Stock,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                TermsUrl = (dto.TermsUrl ?? "").Trim(),
                IsActive = dto.IsActive
            };

            var saved = await _rewardRepository.AddAsync(reward);

            if (!saved)
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not save reward");

            return CreatedAtAction(nameof(GetRewardsById), new { id = reward.Id }, reward);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateReward([FromBody] RewardCreateDto dto, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reward = new Reward
            {
                Title = dto.Title.Trim(),
                Description = (dto.Description ?? "").Trim(),
                CostPoints = dto.CostPoints,
                Stock = dto.Stock,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo,
                TermsUrl = (dto.TermsUrl ?? "").Trim(),
                IsActive = dto.IsActive
            };

            var saved = await _rewardRepository.UpdateAsync(reward, id);
            if (!saved) return NotFound();

            return NoContent();
        }

        //[Authorize(Roles = "Admin")]
        //[HttpGet("admin")]
        //public async Task<ActionResult<IEnumerable<Reward>>> GetAllRewardsAdmin()
        //{
        //    var rewards = await _rewardRepository.GetAllAsync();
        //    return Ok(rewards);
        //}

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllAdmin()
        {
            var rewards = await _rewardRepository.GetAllActiveAsync(); // ή GetAllAsync αν φτιάξεις ξεχωριστό
            return Ok(rewards);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var (ok, error) = await _rewardRepository.DeleteAsyncSafe(id);

            if (ok) return NoContent();

            if (error == "NOT_FOUND")
                return NotFound();

            if (error == "HAS_REDEMPTIONS")
                return Conflict("Δεν μπορεί να διαγραφεί: υπάρχουν εξαργυρώσεις για αυτό το reward.");

            return StatusCode(500, "Delete failed.");
        }

    }
}

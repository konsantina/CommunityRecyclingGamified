using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommunityRecyclingGamified.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedemptionController : ControllerBase
    {
        private readonly IRedemptionRepository _redemptionRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IRewardRepository _rewardRepository;

        public RedemptionController(
            IRedemptionRepository redemptionRepository,
            IUserProfileRepository userProfileRepository,
            IRewardRepository rewardRepository)
        {
            _redemptionRepository = redemptionRepository;
            _userProfileRepository = userProfileRepository;
            _rewardRepository = rewardRepository;
        }

        // POST: api/Redemption
        [HttpPost]
        public async Task<ActionResult<Redemption>> CreateRedemption([FromBody] RedemptionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1️⃣ Φέρε User
            var user = await _userProfileRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found");

            // 2️⃣ Φέρε Reward
            var reward = await _rewardRepository.GetByIdAsync(dto.RewardId);
            if (reward == null)
                return NotFound("Reward not found");

            // 3️⃣ Business checks
            if (!reward.IsActive)
                return BadRequest("Reward is not active");

            if (reward.ValidFrom.HasValue && reward.ValidFrom > DateTime.UtcNow)
                return BadRequest("Reward not yet available");

            if (reward.ValidTo.HasValue && reward.ValidTo < DateTime.UtcNow)
                return BadRequest("Reward has expired");

            if (reward.Stock.HasValue && reward.Stock <= 0)
                return BadRequest("Reward out of stock");

            if (user.TotalPoints < reward.CostPoints)
                return BadRequest("Not enough points");

            // 4️⃣ Δημιουργία Redemption
            var redemption = new Redemption
            {
                UserId = user.Id,
                RewardId = reward.Id,
                CostSnapshot = reward.CostPoints,
                Status = RedemptionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _redemptionRepository.AddAsync(redemption);

            if (!saved)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not create redemption");

            return CreatedAtAction(
                nameof(GetById),
                new { id = redemption.Id },
                redemption);
        }

        // GET: api/Redemption/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Redemption>> GetById(int id)
        {
            var redemption = await _redemptionRepository.GetByIdAsync(id);
            if (redemption == null)
                return NotFound();

            return Ok(redemption);
        }
    }
}

using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityRecyclingGamified.Controllers
{
    [Authorize]
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

        /// <summary>
        /// User requests a reward redemption.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        [ProducesResponseType(typeof(Redemption), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Redemption>> CreateRedemption([FromBody] RedemptionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1) Get User
            var user = await _userProfileRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                return NotFound("User not found");

            // 2) Get Reward
            var reward = await _rewardRepository.GetByIdAsync(dto.RewardId);
            if (reward == null)
                return NotFound("Reward not found");

            // 3) Business checks
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

            // 4) Create Redemption
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
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not create redemption");

            return CreatedAtAction(nameof(GetById), new { id = redemption.Id }, redemption);
        }

        /// <summary>
        /// Get redemption by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(Redemption), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Redemption>> GetById(int id)
        {
            var redemption = await _redemptionRepository.GetByIdAsync(id);
            if (redemption == null)
                return NotFound();

            return Ok(redemption);
        }

        /// <summary>
        /// Approve a pending redemption (Admin only).
        /// </summary>
        [HttpPost("{id:int}/approve")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Approve(int id, [FromBody] RedemptionApproveDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _redemptionRepository.ApproveAsync(id, dto.ApproverUserId);

            if (!success)
                return BadRequest("Cannot approve redemption (not found / not pending / not enough points / invalid reward).");

            return NoContent();
        }

        /// <summary>
        /// Get all pending redemptions (Admin only).
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<Redemption>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<Redemption>>> GetPending()
        {
            var redemptions = await _redemptionRepository.GetPendingAsync();
            return Ok(redemptions);
        }

        /// <summary>
        /// Get redemptions for a given user (Authenticated).
        /// </summary>
        [HttpGet("user/{userId:int}")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Redemption>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Redemption>>> GetByUser(int userId)
        {
            var redemptions = await _redemptionRepository.GetByUserAsync(userId);
            return Ok(redemptions);
        }

        /// <summary>
        /// Reject a pending redemption (Admin only).
        /// </summary>
        [HttpPost("{id:int}/reject")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Reject(int id, [FromBody] RedemptionRejectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = await _redemptionRepository.RejectAsync(id, dto.RejectedByUserId);

            if (!ok)
                return BadRequest("Cannot reject redemption");

            return NoContent();
        }

        /// <summary>
        /// Fulfill an approved redemption (Admin only).
        /// </summary>
        [HttpPost("{id:int}/fulfill")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Fulfill(int id, [FromBody] string? code)
        {
            var ok = await _redemptionRepository.FulfillAsync(id, code);

            if (!ok)
                return BadRequest("Cannot fulfill redemption");

            return NoContent();
        }
    }
}

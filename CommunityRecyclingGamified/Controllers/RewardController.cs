using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;

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
            var reward = await _rewardRepository.GetAllActiveAsync();
            return Ok(reward);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Reward>> GetRewardsById(int id)
        {
            var reward = await _rewardRepository.GetByIdAsync(id);

            return Ok(reward);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Reward>> DeleteByIdAsync(int id)
        {
            var result = await _rewardRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost]

        public async Task<ActionResult<Reward>> AddReward([FromBody] RewardCreateDto reward)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Reward
            {
                Title = reward.Title,
                Description = reward.Description,
                CostPoints = reward.CostPoints,
                Stock = reward.Stock,
                ValidFrom = reward.ValidFrom,
                ValidTo = reward.ValidTo,
                TermsUrl = reward.TermsUrl
            };

            var saved = await _rewardRepository.AddAsync(user);

            if (!saved)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not save user");
            }

            // 201 Created + Location header + σώμα
            return CreatedAtAction(nameof(GetRewardsById), new { id = user.Id }, user);
        }

        [HttpPut("{id:int}")]
       public async Task<ActionResult<Reward>> UpdateReward(RewardCreateDto reward, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Reward
            {
                Title = reward.Title,
                Description = reward.Description,
                CostPoints = reward.CostPoints,
                Stock = reward.Stock,
                ValidFrom = reward.ValidFrom,
                ValidTo = reward.ValidTo,
                TermsUrl = reward.TermsUrl
            };
            var saved = await _rewardRepository.UpdateAsync(user,id);

            if (!saved)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not save user");
            }

            // 201 Created + Location header + σώμα
            return Ok(saved);
        }

    }
}


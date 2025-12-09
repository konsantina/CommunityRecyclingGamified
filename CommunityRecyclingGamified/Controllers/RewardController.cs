using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories;
using CommunityRecyclingGamified.Repositories.Interfaces;
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
            var reward = await _rewardRepository.GetAllActiveAsync();
            return Ok(reward);
        }
    }
}

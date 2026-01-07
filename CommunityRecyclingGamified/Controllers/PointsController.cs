using System.Security.Claims;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityRecyclingGamified.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PointsController : ControllerBase
    {
        private readonly IPointsRepository _pointsRepo;
        public PointsController(IPointsRepository pointsRepo) => _pointsRepo = pointsRepo;

        private int GetUserId()
        {
            var id =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub") ??
                "0";

            return int.TryParse(id, out var userId) ? userId : 0;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyWallet()
        {
            var userId = GetUserId();
            if (userId <= 0) return Unauthorized();

            var wallet = await _pointsRepo.GetWalletAsync(userId);
            if (wallet == null) return NotFound();

            return Ok(wallet);
        }

        [HttpGet("me/ledger")]
        public async Task<IActionResult> GetMyLedger([FromQuery] int days = 30, [FromQuery] int take = 50)
        {
            var userId = GetUserId();
            if (userId <= 0) return Unauthorized();

            var items = await _pointsRepo.GetLedgerAsync(userId, days, take);
            return Ok(items);
        }
    }
}

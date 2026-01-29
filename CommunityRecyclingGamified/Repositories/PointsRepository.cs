using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Repositories
{
    public class PointsRepository : IPointsRepository
    {
        private readonly AppDbContext _context;
        public PointsRepository(AppDbContext context) => _context = context;

        public async Task<PointsWalletDto?> GetWalletAsync(int userId)
        {
            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var earned = await _context.UserPointLedgers
                .Where(x => x.UserId == userId && x.Amount > 0)
                .SumAsync(x => (int?)x.Amount) ?? 0;

            var spentAbs = await _context.UserPointLedgers
                .Where(x => x.UserId == userId && x.Amount < 0)
                .SumAsync(x => (int?)(-x.Amount)) ?? 0;

            return new PointsWalletDto
            {
                UserId = userId,
                AvailablePoints = user.TotalPoints,
                EarnedTotal = earned,
                SpentTotal = spentAbs
            };
        }

        public async Task<List<PointsLedgerItemDto>> GetLedgerAsync(int userId, int days, int take)
        {
            take = Math.Clamp(take, 1, 200);
            days = Math.Clamp(days, 1, 365);

            var from = DateTime.UtcNow.AddDays(-days);

            return await _context.UserPointLedgers
                .Where(x => x.UserId == userId && x.CreatedAt >= from)
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .Select(x => new PointsLedgerItemDto
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    Amount = x.Amount,
                    Reason = x.Reason.ToString(),
                    RefEntityType = x.RefEntityType,
                    RefEntityId = x.RefEntityId
                })
                .ToListAsync();
        }
    }
}

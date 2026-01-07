using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Repositories
{
    public class RewardRepository : IRewardRepository
    {
        private readonly AppDbContext _context;
        public RewardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Reward reward)
        {
            await _context.Rewards.AddAsync(reward);
            return await _context.SaveChangesAsync() > 0;
        }

        // ✅ ADMIN: φέρνει ΟΛΑ
        public async Task<IEnumerable<Reward>> GetAllAsync()
        {
            return await _context.Rewards
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        // ✅ USER: φέρνει μόνο διαθέσιμα (Active + μέσα σε dates + stock>0)
        public async Task<IEnumerable<Reward>> GetAllActiveAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.Rewards
                .Where(r => r.IsActive)
                .Where(r => !r.ValidFrom.HasValue || r.ValidFrom.Value <= now)
                .Where(r => !r.ValidTo.HasValue || r.ValidTo.Value >= now)
                .Where(r => r.Stock > 0) // αν θες stock πάντα required, δες παρακάτω
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

        public async Task<Reward?> GetByIdAsync(int id)
        {
            return await _context.Rewards.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> UpdateAsync(Reward reward, int id)
        {
            var oldReward = await _context.Rewards.FirstOrDefaultAsync(x => x.Id == id);
            if (oldReward == null) return false;

            oldReward.Title = reward.Title;
            oldReward.Description = reward.Description;
            oldReward.CostPoints = reward.CostPoints;
            oldReward.Stock = reward.Stock;
            oldReward.ValidFrom = reward.ValidFrom;
            oldReward.ValidTo = reward.ValidTo;
            oldReward.TermsUrl = reward.TermsUrl;
            oldReward.IsActive = reward.IsActive;

            _context.Rewards.Update(oldReward);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<(bool ok, string? error)> DeleteAsyncSafe(int id)
        {
            var reward = await _context.Rewards.FirstOrDefaultAsync(r => r.Id == id);
            if (reward == null) return (false, "NOT_FOUND");

            var hasRedemptions = await _context.Redemptions.AnyAsync(x => x.RewardId == id);
            if (hasRedemptions) return (false, "HAS_REDEMPTIONS");

            _context.Rewards.Remove(reward);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var reward = await _context.Rewards.FirstOrDefaultAsync(o => o.Id == id);
            if (reward == null) return false;

            _context.Rewards.Remove(reward);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

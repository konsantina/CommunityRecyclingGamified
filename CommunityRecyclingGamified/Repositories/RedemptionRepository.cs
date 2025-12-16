using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Repositories
{
    public class RedemptionRepository : IRedemptionRepository
    {
        private readonly AppDbContext _context;

        public RedemptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Redemption redemption)
        {
            await _context.Redemptions.AddAsync(redemption);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Redemption?> GetByIdAsync(int id)
        {
            return await _context.Redemptions
                .Include(r => r.Reward)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Redemption>> GetByUserAsync(int userId)
        {
            return await _context.Redemptions
                .Include(r => r.Reward)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Redemption>> GetPendingAsync()
        {
            return await _context.Redemptions
                .Include(r => r.Reward)
                .Where(r => r.Status == RedemptionStatus.Pending)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}

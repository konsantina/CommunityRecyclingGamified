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
        public Task<bool> AddAsync(Reward reward)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Reward>> GetAllActiveAsync()
        {
            return await _context.Rewards.ToListAsync();
        }

        public async Task<Reward> GetByIdAsync(int id)
        {
            return await _context.Rewards.FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<bool> UpdateAsync(Reward reward)
        {
            throw new NotImplementedException();
        }
    }
}

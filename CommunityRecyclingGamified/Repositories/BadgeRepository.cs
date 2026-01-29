using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Repositories
{
    public class BadgeRepository : IBadgeRepository
    {
        private readonly AppDbContext _context;

        public BadgeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Badge>> GetActiveBadgesAsync()
        {
            return await _context.Badges
                .AsNoTracking()
                .Where(b => b.IsActive)
                .ToListAsync();
        }
    }
}

using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Repositories
{
    public class UserBadgeRepository : IUserBadgeRepository
    {
        private readonly AppDbContext _context;

        public UserBadgeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int userId, int badgeId)
        {
            return await _context.UserBadges
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId && x.BadgeId == badgeId);
        }

        public async Task<bool> AddAsync(UserBadge userBadge)
        {
            await _context.UserBadges.AddAsync(userBadge);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AppDbContext _context;
        public UserProfileRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddAsync(UserProfile user)
        {
            await _context.UserProfiles.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.Id == id);
            if (userProfile == null)
                return false;

            _context.UserProfiles.Remove(userProfile);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<UserProfile>> GetAllAsync()
        {
            return await _context.UserProfiles.ToListAsync();
        }

        public Task<UserProfile?> GetByEmailAsync(string email)
        {
           return _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<UserProfile?> GetByIdAsync(int id)
        {
            return _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> UpdateAsync(UserProfile user)
        {
            _context.UserProfiles.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<BadgeDto>> GetMyBadgesAsync(int userId)
        {
            return await _context.Badges
                .AsNoTracking()
                .Where(b => b.IsActive)
                .Select(b => new BadgeDto
                {
                    Id = b.Id,
                    Name = b.Name ?? "",
                    Description = b.Description,
                    IconUrl = b.IconUrl,
                    RuleType = b.RuleType,

                    // αν υπάρχει UserBadge για τον χρήστη → unlocked
                    Unlocked = b.UserBadges.Any(ub => ub.UserId == userId),

                    // progress/target για τώρα null (UI-ready)
                    Progress = null,
                    Target = null
                })
                .ToListAsync();
        }
    }
}

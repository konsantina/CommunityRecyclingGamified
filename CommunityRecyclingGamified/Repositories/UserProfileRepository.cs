using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
            // metrics
            var verifiedCount = await _context.Dropoffs
                .CountAsync(d => d.UserId == userId && d.Status == DropoffStatus.Verified);

            var totalVolume = await _context.Dropoffs
                .Where(d => d.UserId == userId && d.Status == DropoffStatus.Verified)
                .SumAsync(d => (decimal?)d.Quantity) ?? 0m;

            var dates = await _context.Dropoffs
                .Where(d => d.UserId == userId &&
                            d.Status == DropoffStatus.Verified &&
                            d.VerifiedAt != null)
                .Select(d => d.VerifiedAt!.Value.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            var streak = CalculateStreakDays(dates);

            var leaderboard = await _context.UserPointLedgers
                .GroupBy(x => x.UserId)
                .Select(g => new { UserId = g.Key, Total = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.Total)
                .ToListAsync();

            var rank = leaderboard.FindIndex(x => x.UserId == userId) + 1;

            var unlocked = await _context.UserBadges
                .Where(x => x.UserId == userId)
                .Select(x => x.BadgeId)
                .ToListAsync();

            var badges = await _context.Badges
                .Where(b => b.IsActive)
                .OrderBy(b => b.Id)
                .ToListAsync();

            return badges.Select(b =>
            {
                var isUnlocked = unlocked.Contains(b.Id);

                int? progress = null;
                int? target = null;

                if (!isUnlocked)
                {
                    switch (b.RuleType)
                    {
                        case BadgeRuleType.Count:
                            progress = verifiedCount;
                            target = ReadInt(b.RuleParams, "count");
                            break;

                        case BadgeRuleType.Volume:
                            progress = (int)Math.Floor(totalVolume);
                            target = ReadInt(b.RuleParams, "min");
                            break;

                        case BadgeRuleType.Streak:
                            progress = streak;
                            target = ReadInt(b.RuleParams, "days");
                            break;

                        case BadgeRuleType.Rank:
                            progress = rank > 0 ? rank : null;
                            target = ReadInt(b.RuleParams, "rank");
                            break;
                    }
                }

                return new BadgeDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    IconUrl = b.IconUrl,
                    RuleType = b.RuleType,
                    Unlocked = isUnlocked,
                    Progress = progress,
                    Target = target
                };
            }).ToList();
        }

        private static int CalculateStreakDays(List<DateTime> dates)
        {
            if (!dates.Any()) return 0;

            var set = dates.Select(d => d.Date).ToHashSet();
            var current = set.Max();

            int streak = 0;
            while (set.Contains(current))
            {
                streak++;
                current = current.AddDays(-1);
            }
            return streak;
        }

        private static int ReadInt(string? json, string key)
        {
            if (string.IsNullOrWhiteSpace(json)) return 0;

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty(key, out var el) && el.TryGetInt32(out var v)
                ? v
                : 0;
        }


    }
}

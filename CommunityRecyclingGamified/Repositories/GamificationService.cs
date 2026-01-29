using System.Text.Json;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using CommunityRecyclingGamified.Services.Interfaces;

namespace CommunityRecyclingGamified.Services
{
    public class GamificationService : IGamificationService
    {
        private readonly IDropoffRepository _dropoffRepo;
        private readonly IBadgeRepository _badgeRepo;
        private readonly IUserBadgeRepository _userBadgeRepo;
        private readonly IUserPointLedgerRepository _ledgerRepo;

        public GamificationService(
            IDropoffRepository dropoffRepo,
            IBadgeRepository badgeRepo,
            IUserBadgeRepository userBadgeRepo,
            IUserPointLedgerRepository ledgerRepo)
        {
            _dropoffRepo = dropoffRepo;
            _badgeRepo = badgeRepo;
            _userBadgeRepo = userBadgeRepo;
            _ledgerRepo = ledgerRepo;
        }

        public async Task OnDropoffVerifiedAsync(int dropoffId)
        {
            // 1) Πάρε το dropoff για να βρούμε userId
            var dropoff = await _dropoffRepo.GetByIdAsync(dropoffId);
            if (dropoff == null) return;

            if (dropoff.Status != DropoffStatus.Verified) return;

            var userId = dropoff.UserId;

            // 2) Πάρε τα ενεργά badges
            var badges = await _badgeRepo.GetActiveBadgesAsync();
            if (badges == null || badges.Count == 0) return;

            // 3) Metrics (μια φορά)
            var verifiedCount = await _dropoffRepo.CountVerifiedByUserAsync(userId);
            var totalVolume = await _dropoffRepo.SumVerifiedVolumeByUserAsync(userId);
            var dates = await _dropoffRepo.GetVerifiedDropoffDatesAsync(userId);
            var streakDays = CalculateStreakDays(dates);

            // 4) Rank (leaderboard)
            var leaderboard = await _ledgerRepo.GetLeaderboardAsync();
            var rank = (leaderboard == null) ? 0 : leaderboard.FindIndex(x => x.UserId == userId) + 1; // 1-based

            // 5) Evaluate & award
            foreach (var badge in badges)
            {
                if (badge == null) continue;

                if (await _userBadgeRepo.ExistsAsync(userId, badge.Id))
                    continue;

                var unlocked = badge.RuleType switch
                {
                    BadgeRuleType.Count => verifiedCount >= ReadInt(badge.RuleParams, "count"),
                    BadgeRuleType.Volume => totalVolume >= ReadDecimal(badge.RuleParams, "min"),
                    BadgeRuleType.Streak => streakDays >= ReadInt(badge.RuleParams, "days"),
                    BadgeRuleType.Rank => rank > 0 && rank <= ReadInt(badge.RuleParams, "rank"),
                    _ => false
                };

                if (!unlocked) continue;

                await _userBadgeRepo.AddAsync(new UserBadge
                {
                    UserId = userId,
                    BadgeId = badge.Id,
                    UnlockedAt = DateTime.UtcNow
                });
            }
        }

        private static int CalculateStreakDays(List<DateOnly>? dates)
        {
            if (dates == null || dates.Count == 0) return 0;

            var set = new HashSet<DateOnly>(dates);
            var current = dates.Max(); 

            var streak = 0;
            while (set.Contains(current))
            {
                streak++;
                current = current.AddDays(-1);
            }
            return streak;
        }

        private static int ReadInt(string? json, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return 0;

                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty(key, out var el)) return 0;

                if (el.ValueKind == JsonValueKind.Number && el.TryGetInt32(out var v)) return v;
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private static decimal ReadDecimal(string? json, string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return 0m;

                using var doc = JsonDocument.Parse(json);
                if (!doc.RootElement.TryGetProperty(key, out var el)) return 0m;

                if (el.ValueKind == JsonValueKind.Number && el.TryGetDecimal(out var v)) return v;
                return 0m;
            }
            catch
            {
                return 0m;
            }
        }
    }
}

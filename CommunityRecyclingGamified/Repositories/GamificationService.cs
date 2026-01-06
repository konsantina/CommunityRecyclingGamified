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
            var dropoff = await _dropoffRepo.GetByIdAsync(dropoffId);
            if (dropoff == null) return;


            var userId = dropoff.UserId;

            var badges = await _badgeRepo.GetActiveBadgesAsync();
            if (badges.Count == 0) return;

            // rank από leaderboard (0 αν δεν βρεθεί)
            var leaderboard = await _ledgerRepo.GetLeaderboardAsync();
            var rank = leaderboard.FindIndex(x => x.UserId == userId) + 1;
            if (rank <= 0) return;

            foreach (var badge in badges)
            {
                if (badge.RuleType != BadgeRuleType.Rank)
                    continue;

                if (await _userBadgeRepo.ExistsAsync(userId, badge.Id))
                    continue;

                // RuleParams: {"rank":3} => Top 3
                var top = ReadInt(badge.RuleParams, "rank");
                if (top <= 0) continue;

                if (rank <= top)
                {
                    await _userBadgeRepo.AddAsync(new UserBadge
                    {
                        UserId = userId,
                        BadgeId = badge.Id,
                        UnlockedAt = DateTime.UtcNow
                    });
                }
            }
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
    }
}

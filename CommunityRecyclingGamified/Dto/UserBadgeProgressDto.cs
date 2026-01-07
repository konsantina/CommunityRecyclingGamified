namespace CommunityRecyclingGamified.Dto
{
    public class UserBadgeProgressDto
    {
        public int BadgeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? IconUrl { get; set; }

        public bool Unlocked { get; set; }
        public DateTime? UnlockedAt { get; set; }

        public decimal? Progress { get; set; }
        public decimal? Target { get; set; }
    }
}

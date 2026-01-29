using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Dto
{
    public class BadgeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }
        public bool Unlocked { get; set; }
        public int? Progress { get; set; }   // optional
        public int? Target { get; set; }     // optional
        public BadgeRuleType RuleType { get; set; }
    }
}

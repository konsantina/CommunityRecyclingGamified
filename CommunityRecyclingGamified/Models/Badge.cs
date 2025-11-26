using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Models
{
    public class Badge
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? IconUrl { get; set; }

        [Required]
        public BadgeRuleType RuleType { get; set; }

        // JSON parameters for rule (e.g. { "count": 10, "period": "Monthly" })
        public string? RuleParams { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}

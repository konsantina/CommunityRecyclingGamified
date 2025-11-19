using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityRecyclingGamified.Models
{
    public class UserBadge
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserProfile? UserProfile { get; set; }

        [Required]
        public int BadgeId { get; set; }
        [ForeignKey(nameof(BadgeId))]
        public Badge? Badge { get; set; }

        [Required]
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string DisplayName { get; set; }

        [Required, MaxLength(120)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public Role Role { get; set; } = Role.User;

        public int? NeighborhoodId { get; set; }
        public Neighborhood Neighborhood { get; set; }

        public int TotalPoints { get; set; } = 0;

        [Required]
        public Level Level { get; set; } = Level.Bronze;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        // Navigations
        public ICollection<Dropoff> Dropoffs { get; set; } = new List<Dropoff>();
        public ICollection<UserPointLedger> LedgerEntries { get; set; } = new List<UserPointLedger>();
        public ICollection<Redemption> Redemptions { get; set; } = new List<Redemption>();
        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}

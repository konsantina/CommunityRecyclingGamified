using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Models
{
    public class Redemption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserProfile UserProfile { get; set; }

        [Required]
        public int RewardId { get; set; }
        [ForeignKey(nameof(RewardId))]
        public Reward Reward { get; set; }

        [Required]
        public int CostSnapshot { get; set; }

        [Required]
        public RedemptionStatus Status { get; set; } = RedemptionStatus.Requested;

        public int? ApprovedBy { get; set; }
        public UserProfile ApprovedByUser { get; set; }

        public DateTime? ApprovedAt { get; set; }
        public DateTime? FulfilledAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        [MaxLength(80)]
        public string Code { get; set; } // coupon/qr code

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

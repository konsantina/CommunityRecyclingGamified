using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Models
{
    public class UserPointLedger
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserProfile UserProfile { get; set; }

        // positive (credit) or negative (debit)
        [Required]
        public int Amount { get; set; }

        [Required]
        public PointReason Reason { get; set; }

        // soft reference to the source entity
        [MaxLength(50)]
        public string RefEntityType { get; set; } // "Dropoff", "Redemption"
        public int? RefEntityId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}

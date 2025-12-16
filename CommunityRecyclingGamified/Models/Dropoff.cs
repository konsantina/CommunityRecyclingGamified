using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Models
{
    public class Dropoff
    {
        [Key]
        public int Id { get; set; }

        // Quantities
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public MaterialUnit Unit { get; set; } // Kg or Pcs

        public int PointsAwarded { get; set; } = 0;

        [MaxLength(150)]
        public string Location { get; set; } // text or "lat,lng"

        [Required]
        public DropoffStatus Status { get; set; } = DropoffStatus.Recorded;

        // Relations
        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public UserProfile UserProfile { get; set; }

        [Required]
        public int MaterialId { get; set; }
        [ForeignKey(nameof(MaterialId))]
        public Material Material { get; set; }

        public int? NeighborhoodId { get; set; }
        public Neighborhood Neighborhood { get; set; }

        public int? VerifiedBy { get; set; }
        public UserProfile VerifiedByUser { get; set; }

        public DateTime? VerifiedAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

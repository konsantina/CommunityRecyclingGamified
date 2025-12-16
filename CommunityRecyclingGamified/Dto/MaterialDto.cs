using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Dto
{
    public class MaterialDto
    {
     
        [Required, MaxLength(60)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        public MaterialUnit Unit { get; set; }

        [Required]
        public decimal PointFactor { get; set; } // points per unit

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}


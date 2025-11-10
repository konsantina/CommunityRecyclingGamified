using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Models
{
    public class Material
    {
        [Key]
        public int Id { get; set; }

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

        public ICollection<Dropoff> Dropoffs { get; set; } = new List<Dropoff>();
    }
}

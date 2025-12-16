using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Models
{
    public class NeighborhoodScore
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NeighborhoodId { get; set; }
        [ForeignKey(nameof(NeighborhoodId))]
        public Neighborhood Neighborhood { get; set; }

        [Required]
        public Period Period { get; set; } // Weekly/Monthly/AllTime

        [Required, MaxLength(20)]
        public string PeriodKey { get; set; } // e.g. "2025-W45", "2025-06", "all"

        [Required]
        public int PointsTotal { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

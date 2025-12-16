using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Models
{
    public class Neighborhood
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; }

        [Required, MaxLength(60)]
        public string Code { get; set; }

        public string AreaGeo { get; set; } // GeoJSON or text

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserProfile> Users { get; set; } = new List<UserProfile>();
        public ICollection<Dropoff> Dropoffs { get; set; } = new List<Dropoff>();
        public ICollection<NeighborhoodScore> Scores { get; set; } = new List<NeighborhoodScore>();
    }
}

using System.ComponentModel.DataAnnotations;
using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Models
{
    public class LevelRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Level LevelName { get; set; }

        [Required]
        public int MinPoints { get; set; }

        public int? MaxPoints { get; set; } // null = open upper bound

        public decimal? RewardMultiplier { get; set; } // optional bonuses

        [Required]
        public bool IsActive { get; set; } = true;
    }
}

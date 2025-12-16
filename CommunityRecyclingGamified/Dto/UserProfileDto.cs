using CommunityRecyclingGamified.Enums;
using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Dto
{
    public class UserProfileDto
    {
        [Required, MaxLength(60)]
        public string DisplayName { get; set; }

        [Required, MaxLength(120)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }
        public int? NeighborhoodId { get; set; }
        public int TotalPoints { get; set; } = 0;

        [Required]
        public Level Level { get; set; } = Level.Bronze;
    }
}

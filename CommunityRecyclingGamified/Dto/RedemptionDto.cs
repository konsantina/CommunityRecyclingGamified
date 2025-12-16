using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Dto
{
    public class RedemptionCreateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RewardId { get; set; }
    }
}

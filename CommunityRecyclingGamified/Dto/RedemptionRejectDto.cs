using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Dto
{
    public class RedemptionRejectDto
    {
        [Required]
        public int RejectedByUserId { get; set; }

    }
}

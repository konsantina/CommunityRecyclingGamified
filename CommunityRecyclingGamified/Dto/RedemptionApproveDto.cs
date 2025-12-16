using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Dto
{
    public class RedemptionApproveDto
    {
        [Required]
        public int ApproverUserId { get; set; }
    }
}

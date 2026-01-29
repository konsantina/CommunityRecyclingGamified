using CommunityRecyclingGamified.Enums;
using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Dto
{
    public class UserPointLedgerDto
    {

        [Required]
        public int UserId { get; set; }

        [Required]
        public int Amount { get; set; } 

        [Required]
        public PointReason Reason { get; set; }

        [MaxLength(50)]
        public string? RefEntityType { get; set; }

        public int? RefEntityId { get; set; }
    }
}

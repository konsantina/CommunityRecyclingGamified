using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Dto
{
    public class RewardCreateDto
    {
        public string? Title { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [Required]
        public int CostPoints { get; set; }

        public int? Stock { get; set; } // null = unlimited

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        [MaxLength(200)]
        public string TermsUrl { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}

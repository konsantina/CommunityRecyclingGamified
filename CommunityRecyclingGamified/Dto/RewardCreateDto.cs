using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Dto
{
    public class RewardCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [MaxLength(300)]
        public string? Description { get; set; }

        [Required]
        public int CostPoints { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }


        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        [MaxLength(200)]
        public string? TermsUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

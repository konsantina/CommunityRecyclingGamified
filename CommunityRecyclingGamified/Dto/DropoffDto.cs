using CommunityRecyclingGamified.Enums;
using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Dto
{
    public class DropoffDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        public int? NeighborhoodId { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public MaterialUnit Unit { get; set; }

        [MaxLength(150)]
        public string? Location { get; set; }
    }
}


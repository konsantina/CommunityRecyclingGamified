using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Dto
{
    public class DropoffUpdateDto
    {
        public int MaterialId { get; set; }
        public int NeighborhoodId { get; set; }
        public decimal Quantity { get; set; }
        public MaterialUnit Unit { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}

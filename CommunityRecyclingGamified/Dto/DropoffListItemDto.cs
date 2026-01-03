
using System;
using CommunityRecyclingGamified.Enums;

namespace CommunityRecyclingGamified.Dto
{
    public class DropoffListItemDto
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public MaterialUnit Unit { get; set; }
        public int PointsAwarded { get; set; }
        public DropoffStatus Status { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }

        // για UI labels (εύκολο)
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }

        public int? NeighborhoodId { get; set; }
        public string? NeighborhoodName { get; set; }
    }
}



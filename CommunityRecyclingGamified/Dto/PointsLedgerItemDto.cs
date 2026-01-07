namespace CommunityRecyclingGamified.Dto
{
    public class PointsLedgerItemDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public int Amount { get; set; }           
        public string Reason { get; set; } = "";

        public string? RefEntityType { get; set; } 
        public int? RefEntityId { get; set; }
    }
}

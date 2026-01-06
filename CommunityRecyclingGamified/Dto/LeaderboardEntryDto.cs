namespace CommunityRecyclingGamified.Dto
{
    public class LeaderboardEntryDto
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string NeighborhoodName { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
    }
}

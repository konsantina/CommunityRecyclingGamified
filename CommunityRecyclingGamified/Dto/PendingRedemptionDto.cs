namespace CommunityRecyclingGamified.Dto
{
    public class PendingRedemptionDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = "";   

        public int RewardId { get; set; }
        public string RewardTitle { get; set; } = ""; 

        public string Status { get; set; } = "";
        public int? CostSnapshot { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

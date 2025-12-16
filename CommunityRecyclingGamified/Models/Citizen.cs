namespace CommunityRecyclingGamified.Models
{
    public class Citizen
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        // Navigation property for related RecyclingActivities
    }
}

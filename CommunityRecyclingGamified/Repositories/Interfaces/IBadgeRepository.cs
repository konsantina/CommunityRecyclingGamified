using CommunityRecyclingGamified.Models;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IBadgeRepository
    {
        Task<List<Badge>> GetActiveBadgesAsync();
    }
}

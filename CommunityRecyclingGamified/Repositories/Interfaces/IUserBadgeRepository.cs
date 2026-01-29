using CommunityRecyclingGamified.Models;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IUserBadgeRepository
    {
        Task<bool> ExistsAsync(int userId, int badgeId);
        Task<bool> AddAsync(UserBadge userBadge);
    }
}

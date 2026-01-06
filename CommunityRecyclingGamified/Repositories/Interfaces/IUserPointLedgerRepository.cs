using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Models;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IUserPointLedgerRepository
    {
        Task<IEnumerable<UserPointLedger>> GetByUserAsync(int userId);
        Task<bool> AddAsync(UserPointLedger entry);
        Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int? neighborhoodId = null);
        Task<int> GetTotalPointsAsync(int userId);
        Task<bool> ExistsForDropoffAsync(int userId, int dropoffId);

    }
}

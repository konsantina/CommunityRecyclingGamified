using CommunityRecyclingGamified.Models;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IRedemptionRepository
    {
        Task<Redemption?> GetByIdAsync(int id);
        Task<IEnumerable<Redemption>> GetByUserAsync(int userId);
        Task<IEnumerable<Redemption>> GetPendingAsync();
        Task<bool> AddAsync(Redemption redemption);

        Task<bool> ApproveAsync(int redemptionId, int approverUserId);
        Task<bool> RejectAsync(int redemptionId, int rejectedByUserId);
        Task<bool> FulfillAsync(int redemptionId, string? code);
    }

}

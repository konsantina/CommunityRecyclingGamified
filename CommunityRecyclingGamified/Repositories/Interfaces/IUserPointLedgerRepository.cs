using CommunityRecyclingGamified.Models;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IUserPointLedgerRepository
    {
        Task<IEnumerable<UserPointLedger>> GetByUserAsync(int userId);
        Task<bool> AddAsync(UserPointLedger entry);
    }
}

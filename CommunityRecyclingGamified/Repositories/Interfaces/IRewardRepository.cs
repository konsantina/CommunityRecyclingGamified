using CommunityRecyclingGamified.Models;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IRewardRepository
    {
        Task<IEnumerable<Reward>> GetAllActiveAsync();

        Task<Reward> GetByIdAsync(int id);
        Task<bool> AddAsync(Reward reward);
        Task<bool> UpdateAsync(Reward reward,int id);
        Task<bool> DeleteAsync(int id);

    }
}

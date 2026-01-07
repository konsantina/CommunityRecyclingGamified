using CommunityRecyclingGamified.Models;

public interface IRewardRepository
{
    Task<IEnumerable<Reward>> GetAllActiveAsync();
    Task<IEnumerable<Reward>> GetAllAsync(); 
    Task<Reward?> GetByIdAsync(int id);

    Task<bool> AddAsync(Reward reward);
    Task<bool> UpdateAsync(Reward reward, int id);
    Task<bool> DeleteAsync(int id);
    Task<(bool ok, string? error)> DeleteAsyncSafe(int id);
}

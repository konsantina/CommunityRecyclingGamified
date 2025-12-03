using CommunityRecyclingGamified.Models;

namespace CommunityRecyclingGamified.Repositories
{
    public interface INeighborhoodRepository
    {
        Task<IEnumerable<Neighborhood>> GetAllAsync();
        Task<Neighborhood> GetByIdAsync(int id);
    }
}

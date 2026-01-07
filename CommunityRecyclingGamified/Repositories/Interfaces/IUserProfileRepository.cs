using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<IEnumerable<UserProfile>> GetAllAsync();
        Task<UserProfile?> GetByIdAsync(int id);
        Task<UserProfile?> GetByEmailAsync(string email);
        Task<bool> AddAsync(UserProfile user);
        Task<bool> UpdateAsync(UserProfile user);
        Task<bool> Delete(int id);
        Task<List<BadgeDto>> GetMyBadgesAsync(int userId);

    }
}

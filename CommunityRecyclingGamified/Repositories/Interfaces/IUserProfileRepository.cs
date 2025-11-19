using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityRecyclingGamified.Models;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<IEnumerable<UserProfile>> GetAllAsync();
        Task<UserProfile?> GetByIdAsync(int id);
        Task<UserProfile?> GetByEmailAsync(string email);
        Task AddAsync(UserProfile user);
        void Update(UserProfile user);
        void Delete(UserProfile user);
        Task<bool> SaveChangesAsync();
    }
}

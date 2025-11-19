using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AppDbContext _context;
        public UserProfileRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task AddAsync(UserProfile user)
        {
            throw new NotImplementedException();
        }

        public void Delete(UserProfile user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserProfile>> GetAllAsync()
        {
            return await _context.UserProfiles.ToListAsync();
        }

        public Task<UserProfile?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<UserProfile?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(UserProfile user)
        {
            throw new NotImplementedException();
        }
    }
}

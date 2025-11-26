using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
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
        public async Task<bool> AddAsync(UserProfile user)
        {
            await _context.UserProfiles.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
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
           return _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<UserProfile?> GetByIdAsync(int id)
        {
            return _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == id);
        }

        public void Update(UserProfile user)
        {
            _context.SaveChangesAsync();
        }
    }
}

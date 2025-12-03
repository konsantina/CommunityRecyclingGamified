using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace CommunityRecyclingGamified.Repositories
{
    public class NeighborhoodRepository : INeighborhoodRepository
    {
        private readonly AppDbContext _context;

        public NeighborhoodRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<Neighborhood>> GetAllAsync()
        {
            return await _context.Neighborhoods.ToListAsync();
        }

        public async Task<Neighborhood> GetByIdAsync(int id)
        {
            return await _context.Neighborhoods.FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}

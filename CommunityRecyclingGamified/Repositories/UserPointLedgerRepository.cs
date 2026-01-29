using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class UserPointLedgerRepository : IUserPointLedgerRepository
{
    private readonly AppDbContext _context;

    public UserPointLedgerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserPointLedger>> GetByUserAsync(int userId)
    {
        return await _context.UserPointLedgers
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> AddAsync(UserPointLedger entry)
    {
        await _context.UserPointLedgers.AddAsync(entry);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int? neighborhoodId = null)
    {
        var query = _context.UserPointLedgers
            .AsNoTracking()
            .Include(x => x.UserProfile)
                .ThenInclude(u => u.Neighborhood)
            .AsQueryable();

        if (neighborhoodId.HasValue)
        {
            query = query.Where(x => x.UserProfile.NeighborhoodId == neighborhoodId.Value);
        }

        return await query
            .GroupBy(x => new
            {
                x.UserId,
                x.UserProfile.DisplayName,
                NeighborhoodName = x.UserProfile.Neighborhood.Name
            })
            .Select(g => new LeaderboardEntryDto
            {
                UserId = g.Key.UserId,
                DisplayName = g.Key.DisplayName,
                NeighborhoodName = g.Key.NeighborhoodName,
                TotalPoints = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalPoints)
            .ToListAsync();
    }

    public async Task<int> GetTotalPointsAsync(int userId)
    {
        return await _context.UserPointLedgers
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .SumAsync(x => (int?)x.Amount) ?? 0;
    }

    public async Task<bool> ExistsForDropoffAsync(int userId, int dropoffId)
    {
        return await _context.UserPointLedgers
            .AsNoTracking()
            .AnyAsync(x =>
                x.UserId == userId &&
                x.RefEntityType == "Dropoff" &&
                x.RefEntityId == dropoffId &&
                x.Reason == PointReason.Dropoff
            );
    }
}

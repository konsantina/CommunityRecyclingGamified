using CommunityRecyclingGamified.Data;
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
}

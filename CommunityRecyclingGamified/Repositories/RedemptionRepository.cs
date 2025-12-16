using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Repositories
{
    public class RedemptionRepository : IRedemptionRepository
    {
        private readonly AppDbContext _context;

        public RedemptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Redemption redemption)
        {
            await _context.Redemptions.AddAsync(redemption);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Redemption?> GetByIdAsync(int id)
        {
            return await _context.Redemptions
                .Include(r => r.Reward)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Redemption>> GetByUserAsync(int userId)
        {
            return await _context.Redemptions
                .Include(r => r.Reward)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }


        public async Task<IEnumerable<Redemption>> GetPendingAsync()
        {
            return await _context.Redemptions
                .Include(r => r.UserProfile)
                .Include(r => r.Reward)
                .Where(r => r.Status == RedemptionStatus.Pending)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ApproveAsync(int redemptionId, int approverUserId)
        {
            var redemption = await _context.Redemptions
                .Include(r => r.UserProfile)
                .Include(r => r.Reward)
                .FirstOrDefaultAsync(r => r.Id == redemptionId);

            if (redemption == null)
                return false;

            if (redemption.Status != RedemptionStatus.Pending)
                return false;

            if (!redemption.Reward.IsActive)
                return false;

            if (redemption.Reward.ValidFrom.HasValue && redemption.Reward.ValidFrom > DateTime.UtcNow)
                return false;

            if (redemption.Reward.ValidTo.HasValue && redemption.Reward.ValidTo < DateTime.UtcNow)
                return false;

            if (redemption.Reward.Stock.HasValue && redemption.Reward.Stock <= 0)
                return false;

            // Points check
            var cost = redemption.CostSnapshot;
            if (redemption.UserProfile.TotalPoints < cost)
                return false;

            redemption.UserProfile.TotalPoints -= cost;

            if (redemption.Reward.Stock.HasValue)
                redemption.Reward.Stock -= 1;

            redemption.Status = RedemptionStatus.Approved;
            redemption.ApprovedBy = approverUserId;
            redemption.ApprovedAt = DateTime.UtcNow;

            var ledgerEntry = new UserPointLedger
            {
                UserId = redemption.UserId,
                Amount = -cost,
                Reason = PointReason.RedemptionCost,
                RefEntityType = "Redemption",
                RefEntityId = redemption.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserPointLedgers.Add(ledgerEntry);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RejectAsync(int redemptionId, int rejectedByUserId)
        {
            var redemption = await _context.Redemptions
                .FirstOrDefaultAsync(r => r.Id == redemptionId);

            if (redemption == null)
                return false;

            if (redemption.Status != RedemptionStatus.Pending)
                return false;

            redemption.Status = RedemptionStatus.Rejected;

            redemption.CancelledAt = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> FulfillAsync(int redemptionId, string? code)
        {
            var redemption = await _context.Redemptions
                .FirstOrDefaultAsync(r => r.Id == redemptionId);

            if (redemption == null)
                return false;

            // Fulfill μόνο αν είναι Approved
            if (redemption.Status != RedemptionStatus.Approved)
                return false;

            redemption.Status = RedemptionStatus.Fulfilled;
            redemption.FulfilledAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(code))
                redemption.Code = code;

            return await _context.SaveChangesAsync() > 0;
        }


    }
}

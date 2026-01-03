using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Enums;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Repositories
{
    public class DropoffRepository : IDropoffRepository
    {
        private readonly AppDbContext _context;

        public DropoffRepository(AppDbContext context) { 
           _context = context;
        }
        public async Task<bool> AddAsync(Dropoff dropoff)
        {
           await _context.Dropoffs.AddAsync(dropoff);
               return await  _context.SaveChangesAsync() > 0;
        }

        public async Task<Dropoff?> GetByIdAsync(int id)
        {
            return await _context.Dropoffs.Include(d => d.Material)
                .Include(d => d.UserProfile)
                .Include(d => d.Neighborhood).FirstOrDefaultAsync(o => o.Id == id);
        }

          public async Task<IEnumerable<Dropoff>> GetPendingAsync()
        {
            return await _context.Dropoffs
                .Include(d => d.Material)
                .Include(d => d.UserProfile)
                .Where(d => d.Status == DropoffStatus.Recorded)
                .OrderBy(d => d.CreatedAt)
                .ToListAsync();
        }
        public Task<bool> UpdateAsync(Dropoff dropoff)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> VerifyAsync(int dropoffId, int verifierUserId)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();
            var dropoff = await _context.Dropoffs
                 .Include(d => d.Material)
                 .Include(d => d.UserProfile)
                 .FirstOrDefaultAsync(d => d.Id == dropoffId);

            if (dropoff == null)
                return false;
            if (dropoff.Status != DropoffStatus.Recorded)
                return false;
            if (dropoff.Material == null || !dropoff.Material.IsActive)
                return false;

            // 1. Υπολογισμός πόντων
            var calculated = dropoff.Quantity * dropoff.Material.PointFactor;
            var points = (int)Math.Floor(calculated);
            if (points < 0)
                points = 0;


            // 2. Ενημέρωση Dropoff
            dropoff.PointsAwarded = points;
            dropoff.Status = DropoffStatus.Verified;
            dropoff.VerifiedBy = verifierUserId;
            dropoff.VerifiedAt = DateTime.UtcNow;

            // 3. Ενημέρωση UserProfile
            dropoff.UserProfile.TotalPoints += points;

            // 4. Δημιουργία Ledger Entry
            var ledger = new UserPointLedger
            {
                UserId = dropoff.UserId,
                Amount = points,
                Reason = PointReason.DropoffReward,
                RefEntityType = "Dropoff",
                RefEntityId = dropoff.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _context.UserPointLedgers.AddAsync(ledger);

            var ok = await _context.SaveChangesAsync() > 0;
            if (!ok)
            {
                await tx.RollbackAsync();
                return false;
            }

            await tx.CommitAsync();
            return true;
        
         }

        public async Task<bool> RejectAsync(int dropoffId, int verifierUserId)
        {
            // εδώ δεν αλλάζουμε user/ledger, αλλά κρατάμε same pattern
            var dropoff = await _context.Dropoffs
                .FirstOrDefaultAsync(d => d.Id == dropoffId);

            if (dropoff == null)
                return false;

            if (dropoff.Status != DropoffStatus.Recorded)
                return false;

            dropoff.Status = DropoffStatus.Rejected;
            dropoff.VerifiedBy = verifierUserId;
            dropoff.VerifiedAt = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

       public async Task<IEnumerable<DropoffListItemDto>> GetByUserAsync(int userId)
        {
            return await _context.Dropoffs
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new DropoffListItemDto
                {
                    Id = d.Id,
                    Quantity = d.Quantity,
                    Unit = d.Unit,
                    PointsAwarded = d.PointsAwarded,
                    Status = d.Status,
                    Location = d.Location,
                    CreatedAt = d.CreatedAt,

                    MaterialId = d.MaterialId,
                    MaterialName = d.Material.Name,

                    NeighborhoodId = d.NeighborhoodId,
                    NeighborhoodName = d.Neighborhood != null ? d.Neighborhood.Name : null
                })
                .ToListAsync();
        }

    }
}


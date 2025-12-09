using CommunityRecyclingGamified.Data;
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
            return await _context.Dropoffs.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Dropoff>> GetByUserAsync(int userId)
        {
          return await _context.Dropoffs
              .Where(d => d.UserId == userId)
              .OrderByDescending(d => d.CreatedAt)
              .ToListAsync();
        }

          public async Task<IEnumerable<Dropoff>> GetPendingAsync()
        {
            return await _context.Dropoffs
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
            var dropoff = await _context.Dropoffs
        .Include(d => d.Material)
        .Include(d => d.UserProfile)
        .FirstOrDefaultAsync(d => d.Id == dropoffId);

            if (dropoff == null)
                return false;

            if (dropoff.Status == DropoffStatus.Verified)
                return false;

            // 1. Υπολογισμός πόντων
            var points = (int)Math.Round(dropoff.Quantity * dropoff.Material.PointFactor);

            // 2. Ενημέρωση Dropoff
            dropoff.Status = DropoffStatus.Verified;
            dropoff.PointsAwarded = points;
            dropoff.VerifiedBy = verifierUserId;
            dropoff.VerifiedAt = DateTime.UtcNow;

            // 3. Ενημέρωση UserProfile
            dropoff.UserProfile.TotalPoints += points;

            // 4. Δημιουργία Ledger Entry
            var ledgerEntry = new UserPointLedger
            {
                UserId = dropoff.UserId,
                Amount = points,
                Reason = PointReason.DropoffReward,
                RefEntityType = "Dropoff",
                RefEntityId = dropoff.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserPointLedgers.Add(ledgerEntry);

            // 5. Αποθήκευση
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

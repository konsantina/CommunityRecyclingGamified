using CommunityRecyclingGamified.Models;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IDropoffRepository
    {
        Task<Dropoff?> GetByIdAsync(int id);

        Task<IEnumerable<Dropoff>> GetByUserAsync(int userId);

        Task<IEnumerable<Dropoff>> GetPendingAsync();

        Task<bool> AddAsync(Dropoff dropoff);

        Task<bool> UpdateAsync(Dropoff dropoff); //(θα το χρησιμοποιήσεις για verify / reject)

        Task<bool> VerifyAsync(int dropoffId, int verifierUserId);


    }
}

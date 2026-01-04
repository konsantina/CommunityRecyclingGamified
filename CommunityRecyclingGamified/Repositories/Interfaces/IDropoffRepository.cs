using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Models;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IDropoffRepository
    {
        Task<bool> AddAsync(Dropoff dropoff);
        Task<Dropoff?> GetByIdAsync(int id);
        Task<IEnumerable<Dropoff>> GetPendingAsync();
        Task<bool> UpdateAsync(Dropoff dropoff); //(το χρησιμοποιω για verify / reject)
        Task<bool> VerifyAsync(int dropoffId, int verifierUserId);
        Task<bool> RejectAsync(int dropoffId, int verifierUserId);
        Task<IEnumerable<DropoffListItemDto>> GetByUserAsync(int userId);
        Task<bool> UpdateByOwnerAsync(int id, int userId, DropoffUpdateDto dto);
        Task<bool> DeleteByOwnerAsync(int id, int userId);


    }
}

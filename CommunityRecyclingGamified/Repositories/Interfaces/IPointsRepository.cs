using CommunityRecyclingGamified.Dto;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IPointsRepository
    {
        Task<PointsWalletDto?> GetWalletAsync(int userId);
        Task<List<PointsLedgerItemDto>> GetLedgerAsync(int userId, int days, int take);
    }
}

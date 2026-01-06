namespace CommunityRecyclingGamified.Services.Interfaces
{
    public interface IGamificationService
    {
        Task OnDropoffVerifiedAsync(int dropoffId);
    }
}

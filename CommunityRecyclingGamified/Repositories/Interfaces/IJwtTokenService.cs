namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateToken(int userId, string email, string role);
    }

}

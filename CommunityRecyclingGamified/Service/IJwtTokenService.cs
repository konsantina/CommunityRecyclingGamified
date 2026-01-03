namespace CommunityRecyclingGamified.Service
{
    public interface IJwtTokenService
    {
        string CreateToken(int userId, string email, string role);
    }

}

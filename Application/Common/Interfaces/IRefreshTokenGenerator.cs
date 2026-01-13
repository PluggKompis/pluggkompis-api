namespace Application.Common.Interfaces
{
    public interface IRefreshTokenGenerator
    {
        (string Token, DateTime ExpiresAt) Generate();
    }
}

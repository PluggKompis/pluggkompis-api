using Application.Common.Interfaces;
using System.Security.Cryptography;

namespace Infrastructure.Services.Security
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public (string Token, DateTime ExpiresAt) Generate()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var token = Convert.ToBase64String(randomBytes);
            return (token, DateTime.UtcNow.AddDays(7));
        }
    }
}

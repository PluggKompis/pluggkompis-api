using Domain.Models.Entities.Users;

namespace Application.Common.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);

        Task AddAsync(User user, CancellationToken ct = default);

        Task UpdateAsync(User user, CancellationToken ct = default);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}

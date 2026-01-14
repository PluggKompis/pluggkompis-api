using Application.Common.Interfaces;
using Domain.Models.Entities.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return _context.Users.AnyAsync(u => u.Email == email, ct);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public Task UpdateAsync(User user, CancellationToken ct = default)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _context.SaveChangesAsync(ct);
        }
    }
}

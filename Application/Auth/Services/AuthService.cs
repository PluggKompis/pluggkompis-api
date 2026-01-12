using Application.Auth.Dtos;
using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using Domain.Models.Enums;

namespace Application.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        private static readonly UserRole[] AllowedSelfRegisterRoles =
            { UserRole.Parent, UserRole.Student, UserRole.Volunteer };

        public AuthService(
            IUserRepository users,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IRefreshTokenGenerator refreshTokenGenerator)
        {
            _users = users;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        public async Task<OperationResult<AuthResponseDto>> RegisterAsync(RegisterUserDto dto)
        {
            var email = NormalizeEmail(dto.Email);

            if (await _users.EmailExistsAsync(email))
                return OperationResult<AuthResponseDto>.Failure("Email already exists.");

            if (!AllowedSelfRegisterRoles.Contains(dto.Role))
                return OperationResult<AuthResponseDto>.Failure("Selected role is not allowed.");

            var refresh = _refreshTokenGenerator.Generate();

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = email,
                PasswordHash = _passwordHasher.Hash(dto.Password),

                RefreshToken = refresh.Token,
                RefreshTokenExpiresAt = refresh.ExpiresAt,

                Role = dto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _users.AddAsync(user);
            await _users.SaveChangesAsync();

            var token = _tokenService.GenerateJwt(user);

            return OperationResult<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                RefreshToken = user.RefreshToken ?? string.Empty,
                User = MapUser(user)
            });
        }

        public async Task<OperationResult<AuthResponseDto>> LoginAsync(LoginUserDto dto)
        {
            var email = NormalizeEmail(dto.Email);

            var user = await _users.GetByEmailAsync(email);

            if (user is null)
                return OperationResult<AuthResponseDto>.Failure("Invalid email or password.");

            if (!user.IsActive)
                return OperationResult<AuthResponseDto>.Failure("User account is inactive.");

            if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
                return OperationResult<AuthResponseDto>.Failure("Invalid email or password.");

            var token = _tokenService.GenerateJwt(user);

            var refresh = _refreshTokenGenerator.Generate();
            user.RefreshToken = refresh.Token;
            user.RefreshTokenExpiresAt = refresh.ExpiresAt;

            await _users.UpdateAsync(user);
            await _users.SaveChangesAsync();

            return OperationResult<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                RefreshToken = user.RefreshToken ?? string.Empty,
                User = MapUser(user)
            });
        }

        public async Task<OperationResult<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                return OperationResult<AuthResponseDto>.Failure("Invalid or expired refresh token.");

            var user = await _users.GetByRefreshTokenAsync(dto.RefreshToken);

            if (user is null)
                return OperationResult<AuthResponseDto>.Failure("Invalid or expired refresh token.");

            if (!user.RefreshTokenExpiresAt.HasValue || user.RefreshTokenExpiresAt.Value < DateTime.UtcNow)
                return OperationResult<AuthResponseDto>.Failure("Invalid or expired refresh token.");

            if (!user.IsActive)
                return OperationResult<AuthResponseDto>.Failure("User account is inactive.");

            var token = _tokenService.GenerateJwt(user);

            var newRefresh = _refreshTokenGenerator.Generate();
            user.RefreshToken = newRefresh.Token;
            user.RefreshTokenExpiresAt = newRefresh.ExpiresAt;

            await _users.UpdateAsync(user);
            await _users.SaveChangesAsync();

            return OperationResult<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                RefreshToken = user.RefreshToken ?? string.Empty,
                User = MapUser(user)
            });
        }

        private static string NormalizeEmail(string email)
            => email.Trim().ToLowerInvariant();

        private static UserDtoResponse MapUser(User user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}

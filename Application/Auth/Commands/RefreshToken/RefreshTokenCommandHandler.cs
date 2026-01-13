using Application.Auth.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using MediatR;
using System.Linq.Expressions;

namespace Application.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, OperationResult<AuthResponseDto>>
    {
        private readonly IGenericRepository<User> _users;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IMapper _mapper;

        public RefreshTokenCommandHandler(
            IGenericRepository<User> users,
            ITokenService tokenService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IMapper mapper)
        {
            _users = users;
            _tokenService = tokenService;
            _refreshTokenGenerator = refreshTokenGenerator;
            _mapper = mapper;
        }

        public async Task<OperationResult<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            Expression<Func<User, bool>> predicate = u => u.RefreshToken == request.Dto.RefreshToken;
            var user = (await _users.FindAsync(predicate)).FirstOrDefault();

            if (user is null)
                return OperationResult<AuthResponseDto>.Failure("Invalid refresh token.");

            if (!user.IsActive)
                return OperationResult<AuthResponseDto>.Failure("User is inactive.");

            if (user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return OperationResult<AuthResponseDto>.Failure("Refresh token expired.");

            // rotate refresh token
            var (refreshToken, expiresAt) = _refreshTokenGenerator.Generate();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = expiresAt;

            await _users.UpdateAsync(user);

            var token = _tokenService.GenerateJwt(user);

            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = user.RefreshToken ?? string.Empty,
                User = _mapper.Map<UserDtoResponse>(user)
            };

            return OperationResult<AuthResponseDto>.Success(response);
        }
    }
}

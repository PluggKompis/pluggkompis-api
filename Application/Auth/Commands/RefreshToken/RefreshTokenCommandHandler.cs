using Application.Auth.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler
        : IRequestHandler<RefreshTokenCommand, OperationResult<AuthResponseDto>>
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IMapper _mapper;

        public RefreshTokenCommandHandler(
            IAuthRepository authRepository,
            ITokenService tokenService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IMapper mapper)
        {
            _authRepository = authRepository;
            _tokenService = tokenService;
            _refreshTokenGenerator = refreshTokenGenerator;
            _mapper = mapper;
        }

        public async Task<OperationResult<AuthResponseDto>> Handle(
            RefreshTokenCommand request,
            CancellationToken ct)
        {
            var refreshTokenFromClient = request.Dto.RefreshToken;

            var user = await _authRepository.GetByRefreshTokenAsync(refreshTokenFromClient, ct);
            if (user is null)
                return OperationResult<AuthResponseDto>.Failure("Invalid refresh token.");

            if (!user.IsActive)
                return OperationResult<AuthResponseDto>.Failure("User is inactive.");

            if (user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return OperationResult<AuthResponseDto>.Failure("Refresh token expired.");

            // Rotate refresh token
            var (newRefreshToken, expiresAt) = _refreshTokenGenerator.Generate();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiresAt = expiresAt;

            await _authRepository.UpdateAsync(user, ct);

            var rows = await _authRepository.SaveChangesAsync(ct);
            if (rows <= 0)
                return OperationResult<AuthResponseDto>.Failure("Failed to persist refresh token rotation.");

            var jwt = _tokenService.GenerateJwt(user);

            var response = new AuthResponseDto
            {
                Token = jwt,
                RefreshToken = user.RefreshToken ?? string.Empty,
                User = _mapper.Map<UserDtoResponse>(user)
            };

            return OperationResult<AuthResponseDto>.Success(response);
        }
    }
}

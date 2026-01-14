using Application.Auth.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Auth.Commands.Login
{
    public class LoginUserCommandHandler
        : IRequestHandler<LoginUserCommand, OperationResult<AuthResponseDto>>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IMapper _mapper;

        public LoginUserCommandHandler(
            IAuthRepository authRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IMapper mapper)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _refreshTokenGenerator = refreshTokenGenerator;
            _mapper = mapper;
        }

        public async Task<OperationResult<AuthResponseDto>> Handle(
            LoginUserCommand request,
            CancellationToken ct)
        {
            var dto = request.Dto;

            var user = await _authRepository.GetByEmailAsync(dto.Email, ct);
            if (user is null)
                return OperationResult<AuthResponseDto>.Failure("Invalid credentials.");

            if (!user.IsActive)
                return OperationResult<AuthResponseDto>.Failure("User is inactive.");

            var passwordValid = _passwordHasher.Verify(dto.Password, user.PasswordHash);
            if (!passwordValid)
                return OperationResult<AuthResponseDto>.Failure("Invalid credentials.");

            // Rotate refresh token on login
            var (refreshToken, expiresAt) = _refreshTokenGenerator.Generate();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = expiresAt;

            await _authRepository.UpdateAsync(user, ct);

            var rows = await _authRepository.SaveChangesAsync(ct);
            if (rows <= 0)
                return OperationResult<AuthResponseDto>.Failure("Failed to persist login changes.");

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

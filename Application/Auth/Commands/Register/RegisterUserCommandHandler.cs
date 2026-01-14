using Application.Auth.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using MediatR;

namespace Application.Auth.Commands.Register
{
    public class RegisterUserCommandHandler
        : IRequestHandler<RegisterUserCommand, OperationResult<AuthResponseDto>>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(
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
            RegisterUserCommand request,
            CancellationToken ct)
        {
            var email = request.Dto.Email;

            var emailExists = await _authRepository.EmailExistsAsync(email, ct);
            if (emailExists)
                return OperationResult<AuthResponseDto>.Failure("Email already exists.");

            var user = _mapper.Map<User>(request.Dto);

            user.PasswordHash = _passwordHasher.Hash(request.Dto.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            var (refreshToken, expiresAt) = _refreshTokenGenerator.Generate();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = expiresAt;

            await _authRepository.AddAsync(user, ct);

            var rows = await _authRepository.SaveChangesAsync(ct);
            if (rows <= 0)
                return OperationResult<AuthResponseDto>.Failure("Failed to persist user.");

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

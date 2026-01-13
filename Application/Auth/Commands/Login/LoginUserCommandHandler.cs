using Application.Auth.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using MediatR;
using System.Linq.Expressions;

namespace Application.Auth.Commands.Login
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, OperationResult<AuthResponseDto>>
    {
        private readonly IGenericRepository<User> _users;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IMapper _mapper;

        public LoginUserCommandHandler(
            IGenericRepository<User> users,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IMapper mapper)
        {
            _users = users;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _refreshTokenGenerator = refreshTokenGenerator;
            _mapper = mapper;
        }

        public async Task<OperationResult<AuthResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            Expression<Func<User, bool>> predicate = u => u.Email == request.Dto.Email;
            var user = (await _users.FindAsync(predicate)).FirstOrDefault();

            if (user is null)
                return OperationResult<AuthResponseDto>.Failure("Invalid credentials.");

            if (!user.IsActive)
                return OperationResult<AuthResponseDto>.Failure("User is inactive.");

            if (!_passwordHasher.Verify(request.Dto.Password, user.PasswordHash))
                return OperationResult<AuthResponseDto>.Failure("Invalid credentials.");

            // rotate refresh token on login
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

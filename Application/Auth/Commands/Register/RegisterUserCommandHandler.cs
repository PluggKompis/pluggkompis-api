using Application.Auth.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, OperationResult<AuthResponseDto>>
    {
        private readonly IGenericRepository<User> _users;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(
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

        public async Task<OperationResult<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Business rule: unique email
            Expression<Func<User, bool>> emailPredicate = u => u.Email == request.Dto.Email;
            var existing = (await _users.FindAsync(emailPredicate)).FirstOrDefault();

            if (existing is not null)
                return OperationResult<AuthResponseDto>.Failure("Email already exists.");

            var user = _mapper.Map<User>(request.Dto);

            user.PasswordHash = _passwordHasher.Hash(request.Dto.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            var (refreshToken, expiresAt) = _refreshTokenGenerator.Generate();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = expiresAt;

            await _users.AddAsync(user);

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

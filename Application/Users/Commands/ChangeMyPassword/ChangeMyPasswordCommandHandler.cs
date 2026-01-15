using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using MediatR;

namespace Application.Users.Commands.ChangeMyPassword
{
    public class ChangeMyPasswordCommandHandler : IRequestHandler<ChangeMyPasswordCommand, OperationResult<bool>>
    {
        private readonly IGenericRepository<User> _users;
        private readonly IPasswordHasher _passwordHasher;

        public ChangeMyPasswordCommandHandler(
            IGenericRepository<User> users,
            IPasswordHasher passwordHasher)
        {
            _users = users;
            _passwordHasher = passwordHasher;
        }

        public async Task<OperationResult<bool>> Handle(ChangeMyPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _users.GetByIdAsync(request.UserId);

            if (user is null || !user.IsActive)
                return OperationResult<bool>.Failure("User not found");

            var isValid = _passwordHasher.Verify(request.Dto.CurrentPassword, user.PasswordHash);
            if (!isValid)
                return OperationResult<bool>.Failure("Current password is incorrect");

            user.PasswordHash = _passwordHasher.Hash(request.Dto.NewPassword);

            await _users.UpdateAsync(user);

            return OperationResult<bool>.Success(true);
        }
    }
}

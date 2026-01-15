using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities.Users;
using MediatR;

namespace Application.Users.Commands.SoftDeleteMe
{
    public class SoftDeleteMeCommandHandler : IRequestHandler<SoftDeleteMeCommand, OperationResult>
    {
        private readonly IGenericRepository<User> _users;

        public SoftDeleteMeCommandHandler(IGenericRepository<User> users)
        {
            _users = users;
        }

        public async Task<OperationResult> Handle(SoftDeleteMeCommand request, CancellationToken cancellationToken)
        {
            var user = await _users.GetByIdAsync(request.UserId);

            if (user is null || !user.IsActive)
                return OperationResult.Failure("User not found");

            user.IsActive = false;

            await _users.UpdateAsync(user);

            return OperationResult.Success();
        }
    }
}

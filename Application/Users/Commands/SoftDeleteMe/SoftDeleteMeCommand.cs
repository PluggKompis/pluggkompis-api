using Domain.Models.Common;
using MediatR;

namespace Application.Users.Commands.SoftDeleteMe
{
    public record SoftDeleteMeCommand(Guid UserId) : IRequest<OperationResult>;
}

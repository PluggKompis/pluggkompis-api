using Application.Users.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Users.Commands.ChangeMyPassword
{
    public record ChangeMyPasswordCommand(Guid UserId, ChangePasswordDto Dto) : IRequest<OperationResult<bool>>;
}

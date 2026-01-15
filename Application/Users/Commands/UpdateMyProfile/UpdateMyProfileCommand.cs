using Application.Users.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Users.Commands.UpdateMyProfile
{
    public record UpdateMyProfileCommand(Guid UserId, UpdateMyProfileDto Dto) : IRequest<OperationResult<MyProfileDto>>;
}

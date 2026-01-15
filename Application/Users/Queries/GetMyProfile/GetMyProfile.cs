using Application.Users.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Users.Queries.GetMyProfile
{
    public record GetMyProfileQuery(Guid UserId) : IRequest<OperationResult<MyProfileDto>>;
}

using Application.Volunteers.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Commands.ApproveVolunteerApplication
{
    public record ApproveVolunteerApplicationCommand(
        Guid CoordinatorId,
        Guid ApplicationId,
        ApproveVolunteerRequest Dto)
        : IRequest<OperationResult<VolunteerApplicationDto>>;
}

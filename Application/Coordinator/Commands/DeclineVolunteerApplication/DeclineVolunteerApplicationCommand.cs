using Application.Volunteers.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Commands.DeclineVolunteerApplication
{
    public record DeclineVolunteerApplicationCommand(Guid CoordinatorId, Guid ApplicationId, ApproveVolunteerRequest Dto)
        : IRequest<OperationResult>;
}

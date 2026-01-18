using Application.Volunteers.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Queries.GetPendingApplications
{
    public record GetPendingApplicationsQuery(Guid CoordinatorId) : IRequest<OperationResult<List<VolunteerApplicationDto>>>;
}

using Application.Coordinator.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Queries.GetCoordinatorDashboard
{
    public record GetCoordinatorDashboardQuery(Guid CoordinatorId)
        : IRequest<OperationResult<CoordinatorDashboardDto>>;
}

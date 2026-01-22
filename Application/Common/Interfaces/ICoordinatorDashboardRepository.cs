using Domain.Models.Common;
using Application.Coordinator.Queries.GetCoordinatorDashboard.Models;


namespace Application.Common.Interfaces
{
    public interface ICoordinatorDashboardRepository
    {
        Task<OperationResult<CoordinatorDashboardModel>> BuildCoordinatorDashboardAsync(
            Guid coordinatorId,
            CancellationToken ct);
    }
}

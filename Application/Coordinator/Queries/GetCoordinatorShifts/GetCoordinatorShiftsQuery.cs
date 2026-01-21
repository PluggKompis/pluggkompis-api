using Application.Coordinator.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Queries.GetCoordinatorShifts
{
    public class GetCoordinatorShiftsQuery : IRequest<OperationResult<PagedResult<CoordinatorShiftDto>>>
    {
        public Guid CoordinatorId { get; }

        public DateTime? StartUtc { get; init; }
        public DateTime? EndUtcExclusive { get; init; }

        public bool? IsAttended { get; init; }
        public Guid? VenueId { get; init; }

        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 50;

        public GetCoordinatorShiftsQuery(Guid coordinatorId)
        {
            CoordinatorId = coordinatorId;
        }
    }
}

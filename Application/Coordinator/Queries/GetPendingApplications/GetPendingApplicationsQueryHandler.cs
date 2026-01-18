using Application.Volunteers.Dtos;
using Application.Volunteers.Services;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Queries.GetPendingApplications
{
    public class GetPendingApplicationsQueryHandler
        : IRequestHandler<GetPendingApplicationsQuery, OperationResult<List<VolunteerApplicationDto>>>
    {
        private readonly IVolunteerService _service;

        public GetPendingApplicationsQueryHandler(IVolunteerService service)
        {
            _service = service;
        }

        public Task<OperationResult<List<VolunteerApplicationDto>>> Handle(GetPendingApplicationsQuery request, CancellationToken cancellationToken)
            => _service.GetPendingApplicationsForCoordinatorAsync(request.CoordinatorId);
    }
}

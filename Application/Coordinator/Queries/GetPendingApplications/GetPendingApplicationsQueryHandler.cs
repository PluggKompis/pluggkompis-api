using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Coordinator.Queries.GetPendingApplications
{
    public class GetPendingApplicationsQueryHandler
        : IRequestHandler<GetPendingApplicationsQuery, OperationResult<List<VolunteerApplicationDto>>>
    {
        private readonly IVolunteerApplicationRepository _applications;
        private readonly IMapper _mapper;

        public GetPendingApplicationsQueryHandler(
            IVolunteerApplicationRepository applications,
            IMapper mapper)
        {
            _applications = applications;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<VolunteerApplicationDto>>> Handle(
            GetPendingApplicationsQuery request,
            CancellationToken cancellationToken)
        {
            var apps = await _applications.GetPendingForCoordinatorAsync(request.CoordinatorId);
            var dtos = _mapper.Map<List<VolunteerApplicationDto>>(apps);
            return OperationResult<List<VolunteerApplicationDto>>.Success(dtos);
        }
    }
}

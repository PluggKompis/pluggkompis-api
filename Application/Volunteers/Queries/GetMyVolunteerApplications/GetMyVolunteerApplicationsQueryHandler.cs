using Application.Common.Interfaces;
using Application.Volunteers.Dtos;
using AutoMapper;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Queries.GetMyVolunteerApplications
{
    public class GetMyVolunteerApplicationsQueryHandler
        : IRequestHandler<GetMyVolunteerApplicationsQuery, OperationResult<List<VolunteerApplicationDto>>>
    {
        private readonly IVolunteerApplicationRepository _applications;
        private readonly IMapper _mapper;

        public GetMyVolunteerApplicationsQueryHandler(
            IVolunteerApplicationRepository applications,
            IMapper mapper)
        {
            _applications = applications;
            _mapper = mapper;
        }

        public async Task<OperationResult<List<VolunteerApplicationDto>>> Handle(
            GetMyVolunteerApplicationsQuery request,
            CancellationToken cancellationToken)
        {
            // You likely already have something similar.
            // If not, add a repo method: GetByVolunteerIdAsync(volunteerId)
            var apps = await _applications.GetByVolunteerIdAsync(request.VolunteerId);

            var dtos = _mapper.Map<List<VolunteerApplicationDto>>(apps);
            return OperationResult<List<VolunteerApplicationDto>>.Success(dtos);
        }
    }
}

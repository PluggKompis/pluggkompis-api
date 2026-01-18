using Application.Volunteers.Dtos;
using Application.Volunteers.Services;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Queries.GetMyVolunteerProfile
{
    public class GetMyVolunteerProfileQueryHandler
        : IRequestHandler<GetMyVolunteerProfileQuery, OperationResult<VolunteerProfileDto>>
    {
        private readonly IVolunteerProfileService _service;

        public GetMyVolunteerProfileQueryHandler(IVolunteerProfileService service)
        {
            _service = service;
        }

        public Task<OperationResult<VolunteerProfileDto>> Handle(GetMyVolunteerProfileQuery request, CancellationToken cancellationToken)
            => _service.GetMyProfileAsync(request.VolunteerId);
    }
}

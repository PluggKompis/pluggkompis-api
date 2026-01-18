using Application.Volunteers.Dtos;
using Application.Volunteers.Services;
using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Queries.GetVenueVolunteers
{
    public class GetVenueVolunteersQueryHandler : IRequestHandler<GetVenueVolunteersQuery, OperationResult<List<VolunteerProfileDto>>>
    {
        private readonly IVolunteerService _service;

        public GetVenueVolunteersQueryHandler(IVolunteerService service)
        {
            _service = service;
        }

        public Task<OperationResult<List<VolunteerProfileDto>>> Handle(GetVenueVolunteersQuery request, CancellationToken cancellationToken)
            => _service.GetApprovedVolunteersForVenueAsync(request.VenueId);
    }
}

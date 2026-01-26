using Application.Volunteers.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Venues.Queries.GetVenueVolunteers
{
    public record GetVenueVolunteersQuery(Guid VenueId)
        : IRequest<OperationResult<List<VolunteerProfileDto>>>;
}

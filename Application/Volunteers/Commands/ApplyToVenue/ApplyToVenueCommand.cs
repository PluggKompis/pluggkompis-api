using Application.Volunteers.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Commands.ApplyToVenue
{
    public record ApplyToVenueCommand(Guid VolunteerId, ApplyToVenueRequest Dto) : IRequest<OperationResult<VolunteerProfileDto>>;
}

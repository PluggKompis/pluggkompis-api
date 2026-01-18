using Application.Volunteers.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Queries.GetMyVolunteerProfile
{
    public record GetMyVolunteerProfileQuery(Guid VolunteerId)
        : IRequest<OperationResult<VolunteerProfileDto>>;
}

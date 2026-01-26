using Application.Volunteers.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Commands.UpdateMyVolunteerProfile
{
    public record UpdateMyVolunteerProfileCommand(Guid VolunteerId, UpdateVolunteerProfileRequest Dto)
        : IRequest<OperationResult<VolunteerProfileDto>>;
}

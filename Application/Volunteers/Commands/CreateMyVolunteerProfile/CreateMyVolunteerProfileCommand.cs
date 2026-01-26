using Application.Volunteers.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Commands.CreateMyVolunteerProfile
{
    public record CreateMyVolunteerProfileCommand(Guid VolunteerId, CreateVolunteerProfileRequest Dto)
        : IRequest<OperationResult<VolunteerProfileDto>>;
}

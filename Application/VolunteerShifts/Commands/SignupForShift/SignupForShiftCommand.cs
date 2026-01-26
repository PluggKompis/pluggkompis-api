using Application.VolunteerShifts.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.VolunteerShifts.Commands.SignupForShift
{
    public record SignupForShiftCommand(Guid VolunteerId, CreateShiftSignupRequest Dto)
        : IRequest<OperationResult<VolunteerShiftDto>>;
}

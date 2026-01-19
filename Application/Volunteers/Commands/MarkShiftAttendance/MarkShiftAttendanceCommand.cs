using Application.VolunteerShifts.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Commands.MarkShiftAttendance
{
    public record MarkShiftAttendanceCommand(Guid CoordinatorId, Guid ShiftId, MarkAttendanceRequest Dto)
        : IRequest<OperationResult<Application.VolunteerShifts.Dtos.VolunteerShiftDto>>;
}

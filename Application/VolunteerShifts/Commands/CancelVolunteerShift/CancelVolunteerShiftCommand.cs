using Domain.Models.Common;
using MediatR;

namespace Application.VolunteerShifts.Commands.CancelVolunteerShift
{
    public record CancelVolunteerShiftCommand(Guid VolunteerId, Guid VolunteerShiftId)
        : IRequest<OperationResult>;
}

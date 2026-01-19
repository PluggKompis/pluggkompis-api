using Application.VolunteerShifts.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.VolunteerShifts.Queries.GetTimeSlotVolunteers
{
    public record GetTimeSlotVolunteersQuery(Guid TimeSlotId)
        : IRequest<OperationResult<List<VolunteerShiftDto>>>;
}

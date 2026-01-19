using Application.VolunteerShifts.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.VolunteerShifts.Queries.GetVolunteerUpcomingShifts
{
    public record GetVolunteerUpcomingShiftsQuery(Guid VolunteerId)
        : IRequest<OperationResult<List<VolunteerShiftDto>>>;
}

using Application.VolunteerShifts.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.VolunteerShifts.Queries.GetVolunteerPastShifts
{
    public record GetVolunteerPastShiftsQuery(Guid VolunteerId)
        : IRequest<OperationResult<List<VolunteerShiftDto>>>;
}

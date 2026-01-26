using Application.Volunteers.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Queries.GetAvailableShifts
{
    public record GetAvailableShiftsQuery(Guid VolunteerId)
        : IRequest<OperationResult<List<AvailableShiftDto>>>;
}

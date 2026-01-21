using Application.Common.Dtos;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Queries.ExportMyVolunteerHoursPdf
{
    public record ExportMyVolunteerHoursPdfQuery(
        Guid VolunteerId,
        DateOnly StartDate,
        DateOnly EndDate)
        : IRequest<OperationResult<FileResponseDto>>;
}

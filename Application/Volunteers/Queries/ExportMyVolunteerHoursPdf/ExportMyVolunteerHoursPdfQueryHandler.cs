using Application.Common.Dtos;
using Application.Common.Interfaces;
using Domain.Models.Common;
using MediatR;

namespace Application.Volunteers.Queries.ExportMyVolunteerHoursPdf
{
    public class ExportMyVolunteerHoursPdfQueryHandler
        : IRequestHandler<ExportMyVolunteerHoursPdfQuery, OperationResult<FileResponseDto>>
    {
        private readonly IVolunteerShiftRepository _shifts;

        public ExportMyVolunteerHoursPdfQueryHandler(IVolunteerShiftRepository shifts)
        {
            _shifts = shifts;
        }

        public async Task<OperationResult<FileResponseDto>> Handle(
            ExportMyVolunteerHoursPdfQuery request,
            CancellationToken cancellationToken)
        {
            var startUtc = DateTime.SpecifyKind(
                request.StartDate.ToDateTime(TimeOnly.MinValue),
                DateTimeKind.Utc);

            var endExclusiveUtc = DateTime.SpecifyKind(
                request.EndDate.AddDays(1).ToDateTime(TimeOnly.MinValue),
                DateTimeKind.Utc);

            var shifts = await _shifts.GetAttendedShiftsForVolunteerAsync(
                request.VolunteerId,
                startUtc,
                endExclusiveUtc);

            var pdfBytes = VolunteerHoursPdfGenerator.GeneratePdf(
                shifts,
                request.StartDate,
                request.EndDate);

            var file = new FileResponseDto
            {
                Content = pdfBytes,
                ContentType = "application/pdf",
                FileName = $"volunteer-hours-{request.StartDate:yyyyMMdd}-{request.EndDate:yyyyMMdd}.pdf"
            };

            return OperationResult<FileResponseDto>.Success(file);
        }
    }

    internal static class VolunteerHoursPdfGenerator
    {
        public static byte[] GeneratePdf(
            IEnumerable<object> shifts,
            DateOnly startDate,
            DateOnly endDate)
        {
            return Array.Empty<byte>();
        }
    }
}

using Application.Common.Dtos;
using Application.Common.Interfaces;
using Domain.Models.Common;
using Domain.Models.Entities.Volunteers;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
            IReadOnlyList<VolunteerShift> shifts,
            DateOnly startDate,
            DateOnly endDate)
        {
            // If you run this in dev and get font issues, QuestPDF will still work with default fonts.
            QuestPDF.Settings.License = LicenseType.Community;

            //color theme
            const string DarkGreen = "#063B29";     // tailwind primary.dark
            const string LightStroke = "#E4E3E0";   // neutral-stroke
            const string Muted = "#829D94";         // neutral-secondary

            var volunteerName =
                shifts.FirstOrDefault()?.Volunteer is not null
                    ? $"{shifts.First().Volunteer.FirstName} {shifts.First().Volunteer.LastName}"
                    : "Volunteer";

            var rows = shifts
                .OrderBy(s => s.OccurrenceStartUtc)
                .Select(s =>
                {
                    var venueName = s.TimeSlot?.Venue?.Name ?? "Unknown venue";
                    var date = DateOnly.FromDateTime(s.OccurrenceStartUtc);
                    var start = s.OccurrenceStartUtc;
                    var end = s.OccurrenceEndUtc;

                    var durationHours = Math.Max(0, (end - start).TotalHours);
                    var hoursRounded = Math.Round(durationHours, 2);

                    return new ShiftRow(
                        Date: date,
                        Venue: venueName,
                        TimeRange: $"{start:HH:mm}–{end:HH:mm}",
                        Hours: hoursRounded
                    );
                })
                .ToList();

            var totalHours = Math.Round(rows.Sum(r => r.Hours), 2);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontColor(DarkGreen));

                    page.Header().Element(header =>
                    {
                        header.Column(col =>
                        {
                            // Banner brand (no background)
                            col.Item().Text("PluggKompis")
                                .FontSize(24)
                                .SemiBold()
                                .FontColor(DarkGreen);

                            col.Item().PaddingTop(20);

                            // Report title
                            col.Item().Text("Volunteer Hours Report")
                                .FontSize(18)
                                .SemiBold()
                                .FontColor(DarkGreen);

                            col.Item().Text($"Volunteer: {volunteerName}")
                                .FontSize(12)
                                .FontColor(DarkGreen);

                            col.Item().Text($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}")
                                .FontSize(11)
                                .FontColor(Muted);

                            // Divider (safe replacement for LineHorizontal().LineColor())
                            col.Item()
                                .PaddingTop(16)
                                .BorderBottom(1)
                                .BorderColor(LightStroke)
                                .PaddingBottom(16);
                        });
                    });

                    page.Content().PaddingTop(12).Element(content =>
                    {
                        content.Column(col =>
                        {
                            if (rows.Count == 0)
                            {
                                col.Item().Text("No attended shifts found for the selected period.")
                                    .FontColor(Colors.Grey.Darken2);
                                return;
                            }

                            col.Item().Text("Attended shifts")
                                .SemiBold()
                                .FontSize(12);

                            col.Item().PaddingTop(6).Element(tableContainer =>
                            {
                                tableContainer.Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(90);  // Date
                                        columns.RelativeColumn(3);   // Venue
                                        columns.ConstantColumn(90);  // Time
                                        columns.ConstantColumn(60);  // Hours
                                    });

                                    // Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderCellStyle).Text("Date");
                                        header.Cell().Element(HeaderCellStyle).Text("Venue");
                                        header.Cell().Element(HeaderCellStyle).Text("Time");
                                        header.Cell().Element(HeaderCellStyle).AlignRight().Text("Hours");
                                    });

                                    // Rows
                                    foreach (var r in rows)
                                    {
                                        table.Cell().Element(BodyCellStyle).Text(r.Date.ToString("yyyy-MM-dd"));
                                        table.Cell().Element(BodyCellStyle).Text(r.Venue);
                                        table.Cell().Element(BodyCellStyle).Text(r.TimeRange);
                                        table.Cell().Element(BodyCellStyle).AlignRight().Text(r.Hours.ToString("0.##"));
                                    }

                                    static IContainer HeaderCellStyle(IContainer container) =>
                                        container
                                            .DefaultTextStyle(x => x.SemiBold().FontColor(DarkGreen).FontSize(10))
                                            .PaddingVertical(6)
                                            .PaddingHorizontal(6)
                                            .Background(Colors.Grey.Lighten4)
                                            .BorderBottom(1)
                                            .BorderColor(LightStroke);

                                    static IContainer BodyCellStyle(IContainer container) =>
                                        container
                                            .BorderBottom(1)
                                            .BorderColor(LightStroke)
                                            .PaddingVertical(6)
                                            .PaddingHorizontal(6)
                                            .DefaultTextStyle(x => x.FontColor(DarkGreen));
                                });
                            });

                            col.Item().PaddingTop(10).AlignRight().Text($"Total hours: {totalHours:0.##}")
                                .SemiBold()
                                .FontSize(12);
                        });
                    });
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken2));
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private record ShiftRow(DateOnly Date, string Venue, string TimeRange, double Hours);
    }
}

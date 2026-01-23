using Application.Common.Interfaces;
using Application.Coordinator.Queries.GetCoordinatorDashboard.Models;
using Domain.Models.Common;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CoordinatorDashboardRepository : ICoordinatorDashboardRepository
    {
        private readonly AppDbContext _db;

        public CoordinatorDashboardRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<OperationResult<CoordinatorDashboardModel>> BuildCoordinatorDashboardAsync(
            Guid coordinatorId,
            CancellationToken ct)
        {
            // Resolve venue for coordinator
            var venue = await _db.Venues
                .AsNoTracking()
                .Where(v => v.CoordinatorId == coordinatorId && v.IsActive)
                .Select(v => new { v.Id })
                .FirstOrDefaultAsync(ct);

            if (venue is null)
            {
                return OperationResult<CoordinatorDashboardModel>.Success(
                    new CoordinatorDashboardModel
                    {
                        TotalBookingsThisWeek = 0,
                        TotalVolunteers = 0,
                        UnfilledShiftsCount = 0,
                        UpcomingShifts = new List<UpcomingShiftModel>(),
                        SubjectCoverage = new List<SubjectCoverageModel>(),
                        VolunteerUtilization = new List<VolunteerUtilizationModel>()
                    });
            }

            var venueId = venue.Id;

            var nowUtc = DateTime.UtcNow;

            // Week: Monday 00:00 -> next Monday 00:00 (UTC)
            var weekStartUtc = StartOfWeekUtc(nowUtc, DayOfWeek.Monday);
            var weekEndUtcExclusive = weekStartUtc.AddDays(7);

            // Upcoming range: next 7 days
            var upcomingEndUtcExclusive = nowUtc.AddDays(7);

            // 1) TotalBookingsThisWeek
            // Using BookingDate (your model). If BookingDate is "local date", this is still consistent week-by-week.
            var totalBookingsThisWeek = await _db.Bookings
                .AsNoTracking()
                .Where(b =>
                    b.TimeSlot.VenueId == venueId &&
                    b.Status != BookingStatus.Cancelled &&
                    b.BookingDate >= weekStartUtc &&
                    b.BookingDate < weekEndUtcExclusive)
                .CountAsync(ct);

            // 2) TotalVolunteers (approved volunteer applications for venue)
            var totalVolunteers = await _db.VolunteerApplications
                .AsNoTracking()
                .Where(a => a.VenueId == venueId && a.Status == VolunteerApplicationStatus.Approved)
                .Select(a => a.VolunteerId)
                .Distinct()
                .CountAsync(ct);

            // 3) UpcomingShifts (next 7 days) with volunteer assignments
            var upcomingShiftRows = await _db.VolunteerShifts
                .AsNoTracking()
                .Where(s =>
                    s.TimeSlot.VenueId == venueId &&
                    s.Status != VolunteerShiftStatus.Cancelled &&
                    s.OccurrenceStartUtc >= nowUtc &&
                    s.OccurrenceStartUtc < upcomingEndUtcExclusive)
                .Select(s => new
                {
                    s.TimeSlotId,
                    s.OccurrenceStartUtc,
                    s.OccurrenceEndUtc,
                    VolunteerName = s.Volunteer.FirstName + " " + s.Volunteer.LastName
                })
                .ToListAsync(ct);

            var upcomingShiftModels = upcomingShiftRows
                .GroupBy(x => new { x.TimeSlotId, x.OccurrenceStartUtc, x.OccurrenceEndUtc })
                .Select(g => new UpcomingShiftModel
                {
                    TimeSlotId = g.Key.TimeSlotId,
                    StartUtc = g.Key.OccurrenceStartUtc,
                    EndUtc = g.Key.OccurrenceEndUtc,
                    VolunteersCount = g.Select(x => x.VolunteerName).Distinct().Count(),
                    VolunteerNames = g.Select(x => x.VolunteerName).Distinct().OrderBy(n => n).ToList(),
                })
                .OrderBy(x => x.StartUtc)
                .ToList();

            var upcomingTimeSlotIds = upcomingShiftModels.Select(x => x.TimeSlotId).Distinct().ToList();

            // 4) Add BookingsCount per upcoming shift occurrence
            // We match bookings using (TimeSlotId + BookingDate.Date == StartUtc.Date)
            var bookingCounts = await _db.Bookings
                .AsNoTracking()
                .Where(b =>
                    b.TimeSlot.VenueId == venueId &&
                    b.Status != BookingStatus.Cancelled &&
                    upcomingTimeSlotIds.Contains(b.TimeSlotId))
                .GroupBy(b => new { b.TimeSlotId, Day = b.BookingDate.Date })
                .Select(g => new { g.Key.TimeSlotId, g.Key.Day, Count = g.Count() })
                .ToListAsync(ct);

            // 5) Add subject names per timeslot
            var timeSlotSubjects = await _db.Set<TimeSlotSubject>()
                .AsNoTracking()
                .Where(x => upcomingTimeSlotIds.Contains(x.TimeSlotId))
                .Select(x => new { x.TimeSlotId, SubjectName = x.Subject.Name })
                .ToListAsync(ct);

            foreach (var shift in upcomingShiftModels)
            {
                var day = shift.StartUtc.Date;

                shift.BookingsCount = bookingCounts
                    .Where(x => x.TimeSlotId == shift.TimeSlotId && x.Day == day)
                    .Select(x => x.Count)
                    .FirstOrDefault();

                shift.SubjectNames = timeSlotSubjects
                    .Where(x => x.TimeSlotId == shift.TimeSlotId)
                    .Select(x => x.SubjectName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }

            // 6) UnfilledShiftsCount
            // Definition: timeslots that have NO volunteer shift occurrence in next 7 days.
            // We count only Open (you can include Full too if you still care about volunteer coverage on full sessions).
            var unfilledShiftsCount = await _db.TimeSlots
                .AsNoTracking()
                .Where(ts => ts.VenueId == venueId && ts.Status == TimeSlotStatus.Open)
                .Where(ts =>
                    !_db.VolunteerShifts.Any(vs =>
                        vs.TimeSlotId == ts.Id &&
                        vs.Status != VolunteerShiftStatus.Cancelled &&
                        vs.OccurrenceStartUtc >= nowUtc &&
                        vs.OccurrenceStartUtc < upcomingEndUtcExclusive))
                .CountAsync(ct);

            // 7) SubjectCoverage (approved volunteers at venue grouped by subject)
            // Venue approved volunteers -> VolunteerSubject join -> Subject name
            var subjectCoverage = await _db.VolunteerApplications
                .AsNoTracking()
                .Where(a => a.VenueId == venueId && a.Status == VolunteerApplicationStatus.Approved)
                .Join(_db.Set<VolunteerSubject>().AsNoTracking(),
                    app => app.VolunteerId,
                    vs => vs.VolunteerId,
                    (app, vs) => new { vs.SubjectId, app.VolunteerId })
                .Join(_db.Subjects.AsNoTracking(),
                    x => x.SubjectId,
                    s => s.Id,
                    (x, s) => new { SubjectName = s.Name, x.VolunteerId })
                .GroupBy(x => x.SubjectName)
                .Select(g => new SubjectCoverageModel
                {
                    SubjectName = g.Key,
                    VolunteersCount = g.Select(x => x.VolunteerId).Distinct().Count()
                })
                .OrderByDescending(x => x.VolunteersCount)
                .ThenBy(x => x.SubjectName)
                .ToListAsync(ct);

            // 8) VolunteerUtilization (hours per volunteer this week)
            var utilizationRows = await _db.VolunteerShifts
                .AsNoTracking()
                .Where(s =>
                    s.TimeSlot.VenueId == venueId &&
                    s.Status != VolunteerShiftStatus.Cancelled &&
                    s.OccurrenceStartUtc >= weekStartUtc &&
                    s.OccurrenceStartUtc < weekEndUtcExclusive)
                .Select(s => new
                {
                    s.VolunteerId,
                    VolunteerName = s.Volunteer.FirstName + " " + s.Volunteer.LastName,
                    DurationMinutes = EF.Functions.DateDiffMinute(s.OccurrenceStartUtc, s.OccurrenceEndUtc)
                })
                .ToListAsync(ct);

            var volunteerUtilization = utilizationRows
                .GroupBy(x => new { x.VolunteerId, x.VolunteerName })
                .Select(g => new VolunteerUtilizationModel
                {
                    VolunteerId = g.Key.VolunteerId,
                    VolunteerName = g.Key.VolunteerName,
                    HoursThisWeek = Math.Round(g.Sum(x => x.DurationMinutes) / 60.0, 2)
                })
                .OrderByDescending(x => x.HoursThisWeek)
                .ThenBy(x => x.VolunteerName)
                .ToList();

            var model = new CoordinatorDashboardModel
            {
                TotalBookingsThisWeek = totalBookingsThisWeek,
                TotalVolunteers = totalVolunteers,
                UnfilledShiftsCount = unfilledShiftsCount,
                SubjectCoverage = subjectCoverage,
                UpcomingShifts = upcomingShiftModels,
                VolunteerUtilization = volunteerUtilization
            };

            return OperationResult<CoordinatorDashboardModel>.Success(model);
        }

        private static DateTime StartOfWeekUtc(DateTime utcDateTime, DayOfWeek startOfWeek)
        {
            var date = utcDateTime.Date;
            var diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
            return date.AddDays(-diff);
        }
    }
}

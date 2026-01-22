using Application.Common.Interfaces;
using Application.Volunteers.Queries.GetAvailableShifts.Models;
using Domain.Models.Common;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AvailableShiftsRepository : IAvailableShiftsRepository
    {
        private readonly AppDbContext _db;

        public AvailableShiftsRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<OperationResult<List<AvailableShiftModel>>> GetAvailableShiftsAsync(
            Guid volunteerId,
            CancellationToken ct)
        {
            var nowUtc = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(nowUtc);
            var maxWindowDays = 28; // "next 2-4 weeks"
            var windowEnd = today.AddDays(maxWindowDays);

            // Venues where volunteer is approved
            var approvedVenueIds = await _db.VolunteerApplications
                .AsNoTracking()
                .Where(a => a.VolunteerId == volunteerId && a.Status == VolunteerApplicationStatus.Approved)
                .Select(a => a.VenueId)
                .Distinct()
                .ToListAsync(ct);

            if (approvedVenueIds.Count == 0)
                return OperationResult<List<AvailableShiftModel>>.Success(new List<AvailableShiftModel>());

            // Exclude if volunteer already has a future shift for this timeslot
            var alreadySignedUpTimeSlotIds = await _db.VolunteerShifts
                .AsNoTracking()
                .Where(vs =>
                    vs.VolunteerId == volunteerId &&
                    vs.Status != VolunteerShiftStatus.Cancelled &&
                    vs.OccurrenceStartUtc >= nowUtc)
                .Select(vs => vs.TimeSlotId)
                .Distinct()
                .ToListAsync(ct);

            // Pull candidate timeslots (Open + approved venue)
            // We filter "future-ish" here with lightweight logic; recurring requires computed next occurrence.
            var candidates = await _db.TimeSlots
                .AsNoTracking()
                .Where(ts =>
                    approvedVenueIds.Contains(ts.VenueId) &&
                    ts.Status == TimeSlotStatus.Open &&
                    !alreadySignedUpTimeSlotIds.Contains(ts.Id))
                .Select(ts => new
                {
                    TimeSlot = ts,
                    VenueName = ts.Venue.Name,
                    VenueAddress = ts.Venue.Address,
                    VenueCity = ts.Venue.City
                })
                .ToListAsync(ct);

            if (candidates.Count == 0)
                return OperationResult<List<AvailableShiftModel>>.Success(new List<AvailableShiftModel>());

            var timeSlotIds = candidates.Select(x => x.TimeSlot.Id).ToList();

            // Subjects lookup
            var subjectsLookup = await _db.Set<TimeSlotSubject>()
                .AsNoTracking()
                .Where(x => timeSlotIds.Contains(x.TimeSlotId))
                .Select(x => new { x.TimeSlotId, SubjectName = x.Subject.Name })
                .ToListAsync(ct);

            // Build models + compute SortKeyUtc + filter future window
            var result = new List<AvailableShiftModel>();

            foreach (var row in candidates)
            {
                var ts = row.TimeSlot;

                // Filter for future:
                // - one-time: SpecificDate must be >= today and within window
                // - recurring: include only if next occurrence is within window
                DateTime sortKeyUtc;
                if (!ts.IsRecurring)
                {
                    if (ts.SpecificDate is null)
                        continue;

                    if (ts.SpecificDate.Value < today)
                        continue;

                    if (ts.SpecificDate.Value > windowEnd)
                        continue;

                    sortKeyUtc = ToUtcDateTime(ts.SpecificDate.Value, ts.StartTime);
                }
                else
                {
                    var next = GetNextOccurrenceDate(today, ts.DayOfWeek);
                    if (next is null)
                        continue;

                    if (next.Value > windowEnd)
                        continue;

                    sortKeyUtc = ToUtcDateTime(next.Value, ts.StartTime);
                }

                var subjects = subjectsLookup
                    .Where(x => x.TimeSlotId == ts.Id)
                    .Select(x => x.SubjectName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                result.Add(new AvailableShiftModel
                {
                    TimeSlotId = ts.Id,
                    VenueId = ts.VenueId,
                    VenueName = row.VenueName,
                    VenueAddress = row.VenueAddress,
                    VenueCity = row.VenueCity,
                    DayOfWeek = ts.DayOfWeek,
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    IsRecurring = ts.IsRecurring,
                    SpecificDate = ts.SpecificDate,
                    Subjects = subjects,

                    // Optional fields: you don’t have these in model yet, keep null for now
                    VolunteersNeeded = null,
                    VolunteersSignedUp = null,

                    SortKeyUtc = sortKeyUtc
                });
            }

            var sorted = result
                .OrderBy(x => x.SortKeyUtc)
                .ThenBy(x => x.StartTime)
                .ToList();

            return OperationResult<List<AvailableShiftModel>>.Success(sorted);
        }

        private static DateTime ToUtcDateTime(DateOnly date, TimeSpan startTime)
        {
            var timeOnly = TimeOnly.FromTimeSpan(startTime);
            var localLike = date.ToDateTime(timeOnly, DateTimeKind.Utc);
            return localLike;
        }

        private static DateOnly? GetNextOccurrenceDate(DateOnly fromDate, WeekDay weekDay)
        {
            // Your WeekDay enum: Sunday=0..Saturday=6 matches System.DayOfWeek
            var target = (int)weekDay;
            var start = (int)fromDate.DayOfWeek;

            var delta = (target - start + 7) % 7;
            var next = fromDate.AddDays(delta);

            // If "today" matches the weekday, we consider today as next occurrence.
            return next;
        }
    }
}

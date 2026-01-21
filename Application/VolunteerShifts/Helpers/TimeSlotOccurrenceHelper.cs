using Domain.Models.Entities.Venues;
using Domain.Models.Enums;

namespace Application.VolunteerShifts.Helpers
{
    public static class TimeSlotOccurrenceHelper
    {
        // NOTE:
        // Your DB stores StartTime/EndTime without timezone. We compute using UTC date as baseline.
        // If you later introduce true Stockholm-local scheduling, swap this to use a timezone provider.
        public static (DateTime? startUtc, DateTime? endUtc) GetNextOccurrenceUtc(TimeSlot slot, DateTime utcNow)
        {
            if (utcNow.Kind != DateTimeKind.Utc)
                utcNow = DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);

            if (slot.SpecificDate is not null)
            {
                var date = slot.SpecificDate.Value;

                var startLocal = date.ToDateTime(TimeOnly.FromTimeSpan(slot.StartTime));
                var endLocal = date.ToDateTime(TimeOnly.FromTimeSpan(slot.EndTime));

                // Handle overnight slot (end next day)
                if (slot.EndTime <= slot.StartTime)
                    endLocal = endLocal.AddDays(1);

                var startUtc = DateTime.SpecifyKind(startLocal, DateTimeKind.Utc);
                var endUtc = DateTime.SpecifyKind(endLocal, DateTimeKind.Utc);

                return (startUtc, endUtc);
            }

            if (!slot.IsRecurring)
                return (null, null);

            // Compute next date matching DayOfWeek
            var today = DateOnly.FromDateTime(utcNow);
            var target = ToSystemDayOfWeek(slot.DayOfWeek);

            var daysAhead = ((int)target - (int)utcNow.DayOfWeek + 7) % 7;
            var dateCandidate = today.AddDays(daysAhead);

            var startCandidate = dateCandidate.ToDateTime(TimeOnly.FromTimeSpan(slot.StartTime));
            startCandidate = DateTime.SpecifyKind(startCandidate, DateTimeKind.Utc);

            // If it’s “today” but already started, move a week ahead
            if (daysAhead == 0 && startCandidate <= utcNow)
            {
                dateCandidate = dateCandidate.AddDays(7);
                startCandidate = DateTime.SpecifyKind(
                    dateCandidate.ToDateTime(TimeOnly.FromTimeSpan(slot.StartTime)),
                    DateTimeKind.Utc);
            }

            var endCandidate = dateCandidate.ToDateTime(TimeOnly.FromTimeSpan(slot.EndTime));

            // Handle overnight slot (end next day)
            if (slot.EndTime <= slot.StartTime)
                endCandidate = endCandidate.AddDays(1);

            endCandidate = DateTime.SpecifyKind(endCandidate, DateTimeKind.Utc);

            return (startCandidate, endCandidate);
        }

        private static DayOfWeek ToSystemDayOfWeek(WeekDay day)
        {
            return day switch
            {
                WeekDay.Monday => DayOfWeek.Monday,
                WeekDay.Tuesday => DayOfWeek.Tuesday,
                WeekDay.Wednesday => DayOfWeek.Wednesday,
                WeekDay.Thursday => DayOfWeek.Thursday,
                WeekDay.Friday => DayOfWeek.Friday,
                WeekDay.Saturday => DayOfWeek.Saturday,
                WeekDay.Sunday => DayOfWeek.Sunday,
                _ => DayOfWeek.Monday
            };
        }
    }
}

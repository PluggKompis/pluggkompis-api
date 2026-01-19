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
            if (slot.SpecificDate is not null)
            {
                var start = slot.SpecificDate.Value.ToDateTime(TimeOnly.FromTimeSpan(slot.StartTime));
                var end = slot.SpecificDate.Value.ToDateTime(TimeOnly.FromTimeSpan(slot.EndTime));

                // Treat as UTC baseline for now
                start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
                end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

                return (start, end);
            }

            if (!slot.IsRecurring)
                return (null, null);

            // Compute next date matching DayOfWeek
            var today = DateOnly.FromDateTime(utcNow);
            var target = ToSystemDayOfWeek(slot.DayOfWeek);

            // Find next occurrence date
            var daysAhead = ((int)target - (int)utcNow.DayOfWeek + 7) % 7;
            var date = today.AddDays(daysAhead);

            var startCandidate = date.ToDateTime(TimeOnly.FromTimeSpan(slot.StartTime));
            startCandidate = DateTime.SpecifyKind(startCandidate, DateTimeKind.Utc);

            // If it’s “today” but already started, move a week ahead
            if (daysAhead == 0 && startCandidate <= utcNow)
            {
                date = date.AddDays(7);
                startCandidate = DateTime.SpecifyKind(
                    date.ToDateTime(TimeOnly.FromTimeSpan(slot.StartTime)),
                    DateTimeKind.Utc);
            }

            var endCandidate = date.ToDateTime(
                TimeOnly.FromTimeSpan(slot.EndTime));

            return (startCandidate, endCandidate);
        }

        private static DayOfWeek ToSystemDayOfWeek(WeekDay day)
        {
            // Adjust to match your WeekDay enum naming
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

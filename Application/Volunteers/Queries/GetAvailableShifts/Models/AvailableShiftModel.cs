using Domain.Models.Enums;

namespace Application.Volunteers.Queries.GetAvailableShifts.Models
{
    public class AvailableShiftModel
    {
        public Guid TimeSlotId { get; set; }
        public Guid VenueId { get; set; }

        public string VenueName { get; set; } = default!;
        public string? VenueAddress { get; set; }
        public string? VenueCity { get; set; }

        public WeekDay DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public bool IsRecurring { get; set; }
        public DateOnly? SpecificDate { get; set; }

        public List<string> Subjects { get; set; } = new();

        public int? VolunteersNeeded { get; set; }
        public int? VolunteersSignedUp { get; set; }

        // internal sorting helper (not mapped to DTO)
        public DateTime SortKeyUtc { get; set; }
    }
}

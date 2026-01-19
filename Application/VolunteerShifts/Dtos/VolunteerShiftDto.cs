using Domain.Models.Enums;

namespace Application.VolunteerShifts.Dtos
{

    public class VolunteerShiftDto
    {
        public Guid Id { get; set; }

        public Guid TimeSlotId { get; set; }
        public Guid VenueId { get; set; }
        public string? VenueName { get; set; }

        public bool IsRecurring { get; set; }
        public DateOnly? SpecificDate { get; set; }
        public WeekDay DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public VolunteerShiftStatus Status { get; set; }
        public bool IsAttended { get; set; }
        public string? Notes { get; set; }

        // Helpful for “upcoming” UI sorting/filtering
        public DateTime? NextOccurrenceStartUtc { get; set; }
        public DateTime? NextOccurrenceEndUtc { get; set; }

        public double? DurationHours { get; set; }
    }

    public class CreateShiftSignupRequest
    {
        public Guid TimeSlotId { get; set; }
        public string? Notes { get; set; }
    }

}

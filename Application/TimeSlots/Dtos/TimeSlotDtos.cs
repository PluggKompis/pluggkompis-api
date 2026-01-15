using Domain.Models.Enums;

namespace Application.TimeSlots.Dtos
{
    /// <summary>
    /// TimeSlot for list/detail views
    /// </summary>
    public class TimeSlotDto
    {
        public Guid Id { get; set; }
        public Guid VenueId { get; set; }
        public required string VenueName { get; set; }
        public WeekDay DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxStudents { get; set; }
        public int AvailableSpots { get; set; }  // Calculated: MaxStudents - confirmed bookings
        public bool IsRecurring { get; set; }
        public DateOnly? SpecificDate { get; set; }  // For one-time events
        public TimeSlotStatus Status { get; set; }
        public List<string> Subjects { get; set; } = new();  // Subject names
        public int CurrentBookings { get; set; }  // Number of confirmed bookings
    }

    /// <summary>
    /// Create new TimeSlot
    /// </summary>
    public class CreateTimeSlotRequest
    {
        public Guid VenueId { get; set; }
        public WeekDay DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxStudents { get; set; }
        public bool IsRecurring { get; set; } = true; // Default to recurring
        public DateOnly? SpecificDate { get; set; }  // Only if IsRecurring = false
        public List<Guid> SubjectIds { get; set; } = new();
    }

    /// <summary>
    /// Update existing TimeSlot
    /// </summary>
    public class UpdateTimeSlotRequest
    {
        public WeekDay DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxStudents { get; set; }
        public bool IsRecurring { get; set; }
        public DateOnly? SpecificDate { get; set; }
        public TimeSlotStatus Status { get; set; }
        public List<Guid> SubjectIds { get; set; } = new();
    }
}

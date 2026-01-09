using Domain.Models.Entities.Bookings;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Venues
{
    public class TimeSlot
    {
        public Guid Id { get; set; }

        public Guid VenueId { get; set; }
        public Venue Venue { get; set; } = default!;

        public WeekDay DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int MaxStudents { get; set; }
        public bool IsRecurring { get; set; }
        public DateOnly? SpecificDate { get; set; }

        public TimeSlotStatus Status { get; set; }

        // Navigation
        public ICollection<TimeSlotSubject> Subjects { get; set; } = new List<TimeSlotSubject>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<VolunteerShift> VolunteerShifts { get; set; } = new List<VolunteerShift>();
    }
}

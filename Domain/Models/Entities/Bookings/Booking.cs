using Domain.Models.Entities.Children;
using Domain.Models.Entities.Users;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Bookings
{
    public class Booking
    {
        public Guid Id { get; set; }

        public Guid TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = default!;

        // One of these must be set — never both
        public Guid? StudentId { get; set; }
        public User? Student { get; set; }

        public Guid? ChildId { get; set; }
        public Child? Child { get; set; }

        public Guid BookedByUserId { get; set; }
        public User BookedByUser { get; set; } = default!;

        public DateTime BookingDate { get; set; } // Local calendar date of this booked session
        public DateTime BookedAt { get; set; }
        public BookingStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? CancelledAt { get; set; } // When the booking was cancelled, optional
    }
}

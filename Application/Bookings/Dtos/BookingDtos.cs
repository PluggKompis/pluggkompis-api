using Domain.Models.Enums;

namespace Application.Bookings.Dtos
{
    /// <summary>
    /// DTO representing a booking with related information
    /// </summary>
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid TimeSlotId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? ChildId { get; set; }
        public Guid BookedByUserId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime BookedAt { get; set; }
        public BookingStatus Status { get; set; }
        public string? Notes { get; set; }

        // Display Info
        public string VenueName { get; set; } = default!;
        public string? VenueAddress { get; set; }
        public string? VenueCity { get; set; }
        public string TimeSlotTime { get; set; } = default!;  // "16:00 - 18:00"
        public string? ChildName { get; set; }
    }

    /// <summary>
    /// Request DTO for creating a new booking
    /// </summary>
    public class CreateBookingRequest
    {
        public Guid TimeSlotId { get; set; }
        public DateTime BookingDate { get; set; }  // Which specific date (e.g., "2025-01-20")
        public Guid? ChildId { get; set; }  // Optional - only for parents
        public string? Notes { get; set; }
    }
}

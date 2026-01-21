namespace Application.Coordinator.Dtos
{
    public class CoordinatorShiftDto
    {
        public Guid ShiftId { get; set; }

        public Guid TimeSlotId { get; set; }
        public Guid VenueId { get; set; }
        public string VenueName { get; set; } = string.Empty;

        public Guid VolunteerId { get; set; }
        public string VolunteerName { get; set; } = string.Empty;

        public DateTime OccurrenceStartUtc { get; set; }
        public DateTime OccurrenceEndUtc { get; set; }

        public string Status { get; set; } = string.Empty;
        public bool IsAttended { get; set; }
        public string? Notes { get; set; }
    }
}

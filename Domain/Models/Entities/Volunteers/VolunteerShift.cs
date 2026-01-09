using Domain.Models.Entities.Users;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Volunteers
{
    public class VolunteerShift
    {
        public Guid Id { get; set; }

        public Guid TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = default!;

        public Guid VolunteerId { get; set; }
        public User Volunteer { get; set; } = default!;

        public VolunteerShiftStatus Status { get; set; }
        public bool IsAttended { get; set; }
        public string? Notes { get; set; }
    }
}

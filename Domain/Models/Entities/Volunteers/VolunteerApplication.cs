using Domain.Models.Entities.Users;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Volunteers
{
    public class VolunteerApplication
    {
        public Guid Id { get; set; }

        public Guid VolunteerId { get; set; }
        public User Volunteer { get; set; } = default!;

        public Guid VenueId { get; set; }
        public Venue Venue { get; set; } = default!;

        public VolunteerApplicationStatus Status { get; set; } = VolunteerApplicationStatus.Pending;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public Guid? ReviewedByCoordinatorId { get; set; }
        public User? ReviewedByCoordinator { get; set; }

        public DateTime? ReviewedAt { get; set; }
        public string? DecisionNote { get; set; }
    }
}

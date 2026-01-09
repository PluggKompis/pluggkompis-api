using Domain.Models.Entities.Users;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;


namespace Domain.Models.Entities.Volunteers
{
    public class VolunteerApplication
    {
        public Guid Id { get; set; }

        public Guid VenueId { get; set; }
        public Venue Venue { get; set; } = default!;

        public Guid VolunteerUserId { get; set; }
        public User VolunteerUser { get; set; } = default!;

        public VolunteerApplicationStatus Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public string? DecisionNote { get; set; }
        public DateTime? DecidedAtUtc { get; set; }
    }
}

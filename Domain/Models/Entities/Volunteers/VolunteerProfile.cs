using Domain.Models.Entities.Users;
using Domain.Models.Entities.Venues;

namespace Domain.Models.Entities.Volunteers
{
    public class VolunteerProfile
    {
        public Guid VolunteerId { get; set; }
        public User Volunteer { get; set; } = default!;

        public string Bio { get; set; } = default!;
        public string Experience { get; set; } = default!;

        public int? MaxHoursPerWeek { get; set; }

        public Guid? PreferredVenueId { get; set; }
        public Venue? PreferredVenue { get; set; }
    }
}

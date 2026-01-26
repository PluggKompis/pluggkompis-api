using Domain.Models.Entities.Users;
using Domain.Models.Entities.Volunteers;

namespace Domain.Models.Entities.Venues
{
    public class Venue
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string City { get; set; } = default!;
        public string PostalCode { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ContactEmail { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public Guid CoordinatorId { get; set; }
        public User Coordinator { get; set; } = default!;

        public bool IsActive { get; set; }

        // Navigation
        public ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
        public ICollection<VolunteerProfile> PreferredByVolunteers { get; set; } = new List<VolunteerProfile>();
        public ICollection<VolunteerApplication> VolunteerApplications { get; set; } = new List<VolunteerApplication>();
    }
}

using Domain.Models.Entities.Bookings;
using Domain.Models.Entities.Children;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Entities.Venues;
using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;

namespace Domain.Models.Entities.Users
{
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;

        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        // Navigation
        public ICollection<Venue> CoordinatedVenues { get; set; } = new List<Venue>();
        public VolunteerProfile? VolunteerProfile { get; set; }

        public ICollection<Child> Children { get; set; } = new List<Child>();

        public ICollection<Booking> BookingsCreated { get; set; } = new List<Booking>();
        public ICollection<Booking> BookingsAsStudent { get; set; } = new List<Booking>();

        public ICollection<VolunteerShift> VolunteerShifts { get; set; } = new List<VolunteerShift>();
        public ICollection<VolunteerSubject> VolunteerSubjects { get; set; } = new List<VolunteerSubject>();
    }
}

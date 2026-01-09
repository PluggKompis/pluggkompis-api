using Domain.Models.Entities.Bookings;
using Domain.Models.Entities.Users;

namespace Domain.Models.Entities.Children
{
    public class Child
    {
        public Guid Id { get; set; }

        public Guid ParentId { get; set; }
        public User Parent { get; set; } = default!; // is it neccessary to have a Parent navigation property? or just ParentId?

        public string FirstName { get; set; } = default!;
        public int BirthYear { get; set; }
        public string SchoolGrade { get; set; } = default!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

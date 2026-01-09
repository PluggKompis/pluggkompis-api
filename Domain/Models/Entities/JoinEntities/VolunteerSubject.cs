using Domain.Models.Entities.Subjects;
using Domain.Models.Entities.Users;
using Domain.Models.Enums;

namespace Domain.Models.Entities.JoinEntities
{

    // M:N between VolunteerProfile and Subject
    public class VolunteerSubject
    {
        public Guid VolunteerId { get; set; }
        public User Volunteer { get; set; } = default!;

        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; } = default!;

        public ConfidenceLevel ConfidenceLevel { get; set; }
    }
}

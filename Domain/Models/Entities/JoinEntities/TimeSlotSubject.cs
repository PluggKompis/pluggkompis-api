using Domain.Models.Entities.Subjects;
using Domain.Models.Entities.Venues;

namespace Domain.Models.Entities.JoinEntities
{
    public class TimeSlotSubject
    {
        public Guid TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = default!;

        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; } = default!;
    }
}

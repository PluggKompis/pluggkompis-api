using Domain.Models.Entities.JoinEntities;

namespace Domain.Models.Entities.Subjects
{
    public class Subject
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Icon { get; set; }

        public ICollection<VolunteerSubject> VolunteerSubjects { get; set; } = new List<VolunteerSubject>();
        public ICollection<TimeSlotSubject> TimeSlotSubjects { get; set; } = new List<TimeSlotSubject>();
    }
}

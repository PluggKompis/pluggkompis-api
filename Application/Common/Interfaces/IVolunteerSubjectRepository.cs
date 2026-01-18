using Domain.Models.Entities.JoinEntities;

namespace Application.Common.Interfaces
{
    public interface IVolunteerSubjectRepository
    {
        Task ReplaceVolunteerSubjectsAsync(Guid volunteerId, IEnumerable<VolunteerSubject> subjects);
        Task<List<VolunteerSubject>> GetVolunteerSubjectsAsync(Guid volunteerId);
        Task<List<VolunteerSubject>> GetVolunteerSubjectsAsync(IEnumerable<Guid> volunteerIds);
    }
}

using Domain.Models.Entities.Volunteers;

namespace Application.Common.Interfaces
{
    public interface IVolunteerProfileRepository
    {
        Task<VolunteerProfile?> GetByVolunteerIdAsync(Guid volunteerId);
        Task UpsertAsync(VolunteerProfile profile);
    }
}

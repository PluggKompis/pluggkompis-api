using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;

namespace Application.Common.Interfaces
{
    public interface IVolunteerApplicationRepository
    {
        Task<bool> HasApplicationWithStatusAsync(Guid volunteerId, VolunteerApplicationStatus status);
        Task<bool> HasPendingApplicationForVenueAsync(Guid volunteerId, Guid venueId);

        Task AddAsync(VolunteerApplication application);
        Task<VolunteerApplication?> GetByIdAsync(Guid applicationId);

        Task<List<VolunteerApplication>> GetPendingForCoordinatorAsync(Guid coordinatorId);

        Task<List<Guid>> GetApprovedVolunteerIdsForVenueAsync(Guid venueId);

        Task UpdateAsync(VolunteerApplication application);
        Task<List<VolunteerApplication>> GetByVolunteerIdAsync(Guid volunteerId);

    }
}

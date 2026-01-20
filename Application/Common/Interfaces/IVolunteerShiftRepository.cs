using Domain.Models.Entities.Volunteers;

namespace Application.Common.Interfaces
{
    public interface IVolunteerShiftRepository
    {
        Task<VolunteerShift?> GetByIdWithTimeSlotAsync(Guid id);
        Task<VolunteerShift?> GetByVolunteerAndTimeSlotAsync(Guid volunteerId, Guid timeSlotId);
        Task<List<VolunteerShift>> GetUpcomingByVolunteerIdAsync(Guid volunteerId, DateTime utcNow);
        Task<List<VolunteerShift>> GetByTimeSlotIdAsync(Guid timeSlotId);
        Task<VolunteerShift?> GetByIdWithTimeSlotVenueAsync(Guid id);

    }
}

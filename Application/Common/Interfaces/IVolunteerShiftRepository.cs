using Domain.Models.Entities.Volunteers;

namespace Application.Common.Interfaces
{
    public interface IVolunteerShiftRepository
    {
        Task<VolunteerShift?> GetByIdWithTimeSlotAsync(Guid id);
        Task<VolunteerShift?> GetByVolunteerAndTimeSlotAsync(Guid volunteerId, Guid timeSlotId);
        Task<List<VolunteerShift>> GetUpcomingByVolunteerIdAsync(Guid volunteerId, DateTime nowUtc);
        Task<List<VolunteerShift>> GetPastByVolunteerIdAsync(Guid volunteerId, DateTime nowUtc);
        Task<List<VolunteerShift>> GetByTimeSlotIdAsync(Guid timeSlotId);
        Task<VolunteerShift?> GetByIdWithTimeSlotVenueAsync(Guid id);
        Task<List<VolunteerShift>> GetAttendedShiftsForVolunteerAsync(
            Guid volunteerId,
            DateTime startUtc,
            DateTime endExclusiveUtc);
        Task<(List<VolunteerShift> Items, int TotalCount)> GetForCoordinatorAsync(
            Guid coordinatorId,
            DateTime? startUtc,
            DateTime? endUtcExclusive,
            bool? isAttended,
            Guid? venueId,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);
    }
}

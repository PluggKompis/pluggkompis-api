using Domain.Models.Entities.Venues;
using Domain.Models.Enums;

namespace Application.Common.Interfaces
{
    public interface ITimeSlotRepository
    {
        Task<List<TimeSlot>> GetByVenueIdAsync(Guid venueId);
        Task<TimeSlot?> GetByIdAsync(Guid id);
        Task<TimeSlot?> GetByIdWithDetailsAsync(Guid id); // Include Subjects, Bookings

        // Check for overlapping time slots
        Task<bool> HasOverlappingTimeSlotAsync(Guid venueId, WeekDay dayOfWeek, TimeSpan startTime, TimeSpan endTime, Guid? excludeTimeSlotId = null);

        Task<TimeSlot> CreateAsync(TimeSlot timeSlot);
        Task UpdateAsync(TimeSlot timeSlot);
        Task DeleteAsync(Guid id);
    }
}

using Application.Common.Interfaces;
using Domain.Models.Entities.Volunteers;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class VolunteerShiftRepository : IVolunteerShiftRepository
    {
        private readonly AppDbContext _db;

        public VolunteerShiftRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<VolunteerShift?> GetByIdWithTimeSlotAsync(Guid id)
        {
            return await _db.VolunteerShifts
                .Include(x => x.TimeSlot)
                    .ThenInclude(ts => ts.Venue)
                .Include(x => x.Volunteer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<VolunteerShift?> GetByVolunteerAndTimeSlotAsync(Guid volunteerId, Guid timeSlotId)
        {
            return await _db.VolunteerShifts
                .FirstOrDefaultAsync(x => x.VolunteerId == volunteerId && x.TimeSlotId == timeSlotId);
        }

        public async Task<List<VolunteerShift>> GetUpcomingByVolunteerIdAsync(Guid volunteerId, DateTime utcNow)
        {
            // We can’t fully filter upcoming at DB level due to computed “next occurrence” for recurring.
            // So we load relevant shifts and filter in memory.
            var shifts = await _db.VolunteerShifts
                .Include(x => x.TimeSlot)
                    .ThenInclude(ts => ts.Venue)
                .Where(x => x.VolunteerId == volunteerId)
                .ToListAsync();

            // Filter: not cancelled and next occurrence exists and is >= now
            return shifts
                .Where(x => x.Status != Domain.Models.Enums.VolunteerShiftStatus.Cancelled)
                .Where(x =>
                {
                    var (startUtc, _) = Application.VolunteerShifts.Helpers.TimeSlotOccurrenceHelper
                        .GetNextOccurrenceUtc(x.TimeSlot, utcNow);

                    return startUtc is not null && startUtc.Value >= utcNow;
                })
                .ToList();
        }

        public async Task<List<VolunteerShift>> GetByTimeSlotIdAsync(Guid timeSlotId)
        {
            return await _db.VolunteerShifts
                .Include(x => x.Volunteer)
                .Include(x => x.TimeSlot)
                    .ThenInclude(ts => ts.Venue)
                .Where(x => x.TimeSlotId == timeSlotId)
                .ToListAsync();
        }

        public async Task<VolunteerShift?> GetByIdWithTimeSlotVenueAsync(Guid id)
        {
            return await _db.VolunteerShifts
                .Include(x => x.TimeSlot)
                    .ThenInclude(ts => ts.Venue)
                .Include(x => x.Volunteer)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}

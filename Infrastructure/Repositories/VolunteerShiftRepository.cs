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

        public Task<List<VolunteerShift>> GetUpcomingByVolunteerIdAsync(Guid volunteerId, DateTime nowUtc)
            => _db.VolunteerShifts
                .AsNoTracking()
                .Include(x => x.TimeSlot)
                    .ThenInclude(ts => ts.Venue)
                .Where(x =>
                    x.VolunteerId == volunteerId &&
                    x.Status != Domain.Models.Enums.VolunteerShiftStatus.Cancelled &&
                    x.OccurrenceStartUtc >= nowUtc)
                .OrderBy(x => x.OccurrenceStartUtc)
                .ToListAsync();

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

        public Task<List<VolunteerShift>> GetAttendedShiftsForVolunteerAsync(
            Guid volunteerId,
            DateTime startUtc,
            DateTime endExclusiveUtc)
            => _db.VolunteerShifts
                .AsNoTracking()
                .Include(x => x.TimeSlot)
                    .ThenInclude(ts => ts.Venue)
                .Where(x =>
                    x.VolunteerId == volunteerId &&
                    x.IsAttended &&
                    x.OccurrenceStartUtc >= startUtc &&
                    x.OccurrenceStartUtc < endExclusiveUtc)
                .OrderBy(x => x.OccurrenceStartUtc)
                .ToListAsync();
    }
}

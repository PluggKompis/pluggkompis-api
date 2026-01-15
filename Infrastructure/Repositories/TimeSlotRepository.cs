using Application.Common.Interfaces;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository for TimeSlot data access using Entity Framework Core
    /// </summary>
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly AppDbContext _context;

        public TimeSlotRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSlot>> GetByVenueIdAsync(Guid venueId)
        {
            return await _context.TimeSlots
                .Include(ts => ts.Venue)
                .Include(ts => ts.Subjects)
                    .ThenInclude(tss => tss.Subject)
                .Include(ts => ts.Bookings)
                .Where(ts => ts.VenueId == venueId)
                .OrderBy(ts => ts.DayOfWeek)
                .ThenBy(ts => ts.StartTime)
                .ToListAsync();
        }
        public async Task<TimeSlot?> GetByIdAsync(Guid id)
        {
            return await _context.TimeSlots
                .Include(ts => ts.Venue)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }
        public async Task<TimeSlot?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.TimeSlots
                .Include(ts => ts.Venue)
                .Include(ts => ts.Subjects)
                    .ThenInclude(tss => tss.Subject)
                .Include(ts => ts.Bookings)
                .Include(ts => ts.VolunteerShifts)
                    .ThenInclude(vs => vs.Volunteer)
                .FirstOrDefaultAsync(ts => ts.Id == id);
        }
        public async Task<bool> HasOverlappingTimeSlotAsync(Guid venueId, WeekDay dayOfWeek, TimeSpan startTime, TimeSpan endTime, Guid? excludeTimeSlotId = null)
        {
            // Only check overlaps with Open or Full timeslots (not Cancelled)
            var activeStatuses = new[] { TimeSlotStatus.Open, TimeSlotStatus.Full };

            var query = _context.TimeSlots
                .Where(ts => ts.VenueId == venueId
                    && ts.DayOfWeek == dayOfWeek
                    && activeStatuses.Contains(ts.Status));

            if (excludeTimeSlotId.HasValue)
            {
                query = query.Where(ts => ts.Id != excludeTimeSlotId.Value);
            }

            return await query.AnyAsync(ts =>
                ts.StartTime < endTime && ts.EndTime > startTime);
        }

        public async Task<TimeSlot> CreateAsync(TimeSlot timeSlot)
        {
            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();
            return timeSlot;
        }

        public async Task UpdateAsync(TimeSlot timeSlot)
        {
            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot != null)
            {
                _context.TimeSlots.Remove(timeSlot);
                await _context.SaveChangesAsync();
            }
        }
    }
}

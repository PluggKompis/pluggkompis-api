using Application.Common.Interfaces;
using Application.Venues.Dtos;
using Domain.Models.Entities.Venues;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class VenueRepository : IVenueRepository
    {
        /// <summary>
        /// Repository for venue data access using Entity Framework Core
        /// </summary>
        /// 
        private readonly AppDbContext _context;

        public VenueRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Venue>> GetAllAsync(VenueFilterParams filters)
        {
            var query = _context.Venues
                .Include(v => v.Coordinator)
                .AsQueryable();

            // Apply active/inactive filter (default: only show active venues)
            if (filters.IsActive.HasValue)
                query = query.Where(v => v.IsActive == filters.IsActive.Value);

            if (!string.IsNullOrEmpty(filters.City))
                query = query.Where(v => v.City == filters.City);

            // Subject filter requires checking all timeslots and their subjects
            if (filters.SubjectId.HasValue)
            {
                query = query.Where(v => v.TimeSlots
                    .Any(ts => ts.Subjects
                        .Any(tss => tss.SubjectId == filters.SubjectId.Value)));
            }

            // DayOfWeek filter checks if venue has any timeslot on that day
            if (filters.DayOfWeek.HasValue)
            {
                query = query.Where(v => v.TimeSlots
                    .Any(ts => ts.DayOfWeek == filters.DayOfWeek.Value));
            }

            // Pagination
            return await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(VenueFilterParams filters)
        {
            var query = _context.Venues.AsQueryable();

            if (filters.IsActive.HasValue)
                query = query.Where(v => v.IsActive == filters.IsActive.Value);

            if (!string.IsNullOrEmpty(filters.City))
                query = query.Where(v => v.City == filters.City);

            if (filters.SubjectId.HasValue)
            {
                query = query.Where(v => v.TimeSlots
                    .Any(ts => ts.Subjects
                        .Any(tss => tss.SubjectId == filters.SubjectId.Value)));
            }

            if (filters.DayOfWeek.HasValue)
            {
                query = query.Where(v => v.TimeSlots
                    .Any(ts => ts.DayOfWeek == filters.DayOfWeek.Value));
            }

            return await query.CountAsync();
        }

        public async Task<Venue?> GetByIdAsync(Guid id)
        {
            return await _context.Venues
                .Include(v => v.Coordinator)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Venue?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Venues
                .Include(v => v.Coordinator)
                .Include(v => v.TimeSlots)
                    .ThenInclude(ts => ts.Subjects)
                        .ThenInclude(tss => tss.Subject)
                .Include(v => v.TimeSlots)
                    .ThenInclude(ts => ts.VolunteerShifts)
                        .ThenInclude(vs => vs.Volunteer)
                .Include(v => v.PreferredByVolunteers)
                    .ThenInclude(vp => vp.Volunteer)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Venue?> GetByCoordinatorIdAsync(Guid coordinatorId)
        {
            return await _context.Venues
                .Include(v => v.Coordinator)
                .FirstOrDefaultAsync(v => v.CoordinatorId == coordinatorId);
        }

        public async Task<Venue> CreateAsync(Venue venue)
        {
            _context.Venues.Add(venue);
            await _context.SaveChangesAsync();
            return venue;
        }

        public async Task UpdateAsync(Venue venue)
        {
            _context.Venues.Update(venue);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }
        }
    }
}

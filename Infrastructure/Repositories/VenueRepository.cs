using Application.Common.Interfaces;
using Application.Venues.Dtos;
using Domain.Models.Entities.Venues;
using Domain.Models.Enums;
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

            // Apply filters
            query = ApplyFilters(query, filters);

            // Pagination
            return await query
                .OrderBy(v => v.Name)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(VenueFilterParams filters)
        {
            var query = _context.Venues.AsQueryable();

            // Apply filters
            query = ApplyFilters(query, filters);

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
                .Include(v => v.VolunteerApplications)
                    .ThenInclude(va => va.Volunteer)
                        .ThenInclude(vol => vol.VolunteerSubjects)
                            .ThenInclude(vs => vs.Subject)
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

        /// <summary>
        /// Apply filters to venue query (DRY - reused in GetAllAsync and GetCountAsync)
        /// </summary>
        private IQueryable<Venue> ApplyFilters(IQueryable<Venue> query, VenueFilterParams filters)
        {
            // Filter by active/inactive status (default: only show active venues)
            if (filters.IsActive.HasValue)
                query = query.Where(v => v.IsActive == filters.IsActive.Value);

            // Filter by city (exact match)
            if (!string.IsNullOrEmpty(filters.City))
                query = query.Where(v => v.City == filters.City);

            // Filter by multiple subjects (venue must have at least one matching subject)
            if (filters.SubjectIds != null && filters.SubjectIds.Any())
            {
                query = query.Where(v => v.TimeSlots
                    .Any(ts => ts.Subjects
                        .Any(tss => filters.SubjectIds.Contains(tss.SubjectId))));
            }

            // Filter by multiple days (venue must have at least one matching day)
            if (filters.DaysOfWeek != null && filters.DaysOfWeek.Any())
            {
                query = query.Where(v => v.TimeSlots
                    .Any(ts => filters.DaysOfWeek.Contains(ts.DayOfWeek)));
            }

            return query;
        }
    }
}

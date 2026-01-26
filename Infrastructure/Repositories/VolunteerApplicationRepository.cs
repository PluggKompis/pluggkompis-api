using Application.Common.Interfaces;
using Domain.Models.Entities.Volunteers;
using Domain.Models.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class VolunteerApplicationRepository : IVolunteerApplicationRepository
    {
        private readonly AppDbContext _db;

        public VolunteerApplicationRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<bool> HasApplicationWithStatusAsync(Guid volunteerId, VolunteerApplicationStatus status)
            => _db.VolunteerApplications.AnyAsync(x => x.VolunteerId == volunteerId && x.Status == status);

        public Task<bool> HasPendingApplicationForVenueAsync(Guid volunteerId, Guid venueId)
            => _db.VolunteerApplications.AnyAsync(x =>
                x.VolunteerId == volunteerId &&
                x.VenueId == venueId &&
                x.Status == VolunteerApplicationStatus.Pending);

        public async Task AddAsync(VolunteerApplication application)
        {
            await _db.VolunteerApplications.AddAsync(application);
            await _db.SaveChangesAsync();
        }

        public Task<VolunteerApplication?> GetByIdAsync(Guid applicationId)
            => _db.VolunteerApplications
                .Include(x => x.Venue)
                .Include(x => x.Volunteer)
                .FirstOrDefaultAsync(x => x.Id == applicationId);

        public Task<List<VolunteerApplication>> GetPendingForCoordinatorAsync(Guid coordinatorId)
            => _db.VolunteerApplications
                .AsNoTracking()
                .Include(x => x.Venue)
                .Include(x => x.Volunteer)
                .Where(x => x.Status == VolunteerApplicationStatus.Pending
                            && x.Venue.CoordinatorId == coordinatorId)
                .OrderByDescending(x => x.AppliedAt)
                .ToListAsync();

        public Task<List<Guid>> GetApprovedVolunteerIdsForVenueAsync(Guid venueId)
            => _db.VolunteerApplications
                .AsNoTracking()
                .Where(x => x.VenueId == venueId && x.Status == VolunteerApplicationStatus.Approved)
                .Select(x => x.VolunteerId)
                .Distinct()
                .ToListAsync();

        public async Task UpdateAsync(VolunteerApplication application)
        {
            _db.VolunteerApplications.Update(application);
            await _db.SaveChangesAsync();
        }

        public Task<List<VolunteerApplication>> GetByVolunteerIdAsync(Guid volunteerId)
            => _db.VolunteerApplications
                .AsNoTracking()
                .Include(x => x.Venue)
                .Include(x => x.Volunteer)
                .Where(x => x.VolunteerId == volunteerId)
                .OrderByDescending(x => x.AppliedAt)
                .ToListAsync();
    }
}

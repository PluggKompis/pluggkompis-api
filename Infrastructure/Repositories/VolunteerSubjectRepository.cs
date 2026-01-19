using Application.Common.Interfaces;
using Domain.Models.Entities.JoinEntities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class VolunteerSubjectRepository : IVolunteerSubjectRepository
    {
        private readonly AppDbContext _db;

        public VolunteerSubjectRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task ReplaceVolunteerSubjectsAsync(Guid volunteerId, IEnumerable<VolunteerSubject> subjects)
        {
            var existing = await _db.VolunteerSubjects
                .Where(x => x.VolunteerId == volunteerId)
                .ToListAsync();

            _db.VolunteerSubjects.RemoveRange(existing);
            await _db.VolunteerSubjects.AddRangeAsync(subjects);

            await _db.SaveChangesAsync();
        }

        public Task<List<VolunteerSubject>> GetVolunteerSubjectsAsync(Guid volunteerId)
            => _db.VolunteerSubjects
                .AsNoTracking()
                .Include(x => x.Subject)
                .Where(x => x.VolunteerId == volunteerId)
                .ToListAsync();

        public Task<List<VolunteerSubject>> GetVolunteerSubjectsAsync(IEnumerable<Guid> volunteerIds)
            => _db.VolunteerSubjects
                .AsNoTracking()
                .Include(x => x.Subject)
                .Where(x => volunteerIds.Contains(x.VolunteerId))
                .ToListAsync();
    }
}

using Application.Common.Interfaces;
using Domain.Models.Entities.Volunteers;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class VolunteerProfileRepository : IVolunteerProfileRepository
    {
        private readonly AppDbContext _db;

        public VolunteerProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public Task<VolunteerProfile?> GetByVolunteerIdAsync(Guid volunteerId)
            => _db.VolunteerProfiles.FirstOrDefaultAsync(x => x.VolunteerId == volunteerId);

        public async Task UpsertAsync(VolunteerProfile profile)
        {
            var exists = await _db.VolunteerProfiles.AnyAsync(x => x.VolunteerId == profile.VolunteerId);

            if (!exists)
                await _db.VolunteerProfiles.AddAsync(profile);
            else
                _db.VolunteerProfiles.Update(profile);

            await _db.SaveChangesAsync();
        }
    }
}

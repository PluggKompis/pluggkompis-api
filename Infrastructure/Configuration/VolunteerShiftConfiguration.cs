using Domain.Models.Entities.Volunteers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class VolunteerShiftConfiguration : IEntityTypeConfiguration<VolunteerShift>
    {
        public void Configure(EntityTypeBuilder<VolunteerShift> entity)
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.IsAttended).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(2000).IsRequired(false);

            entity.HasIndex(x => x.TimeSlotId);
            entity.HasIndex(x => x.VolunteerId);

            // Indexes
            // Prevent same volunteer signing up twice for same slot
            entity.HasIndex(x => new { x.TimeSlotId, x.VolunteerId }).IsUnique();
            entity.HasIndex(x => x.TimeSlotId);
            entity.HasIndex(x => x.VolunteerId);
            entity.HasIndex(x => x.Status);
        }
    }
}

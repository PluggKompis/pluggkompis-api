using Domain.Models.Entities.Volunteers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class VolunteerProfileConfiguration : IEntityTypeConfiguration<VolunteerProfile>
    {
        public void Configure(EntityTypeBuilder<VolunteerProfile> entity)
        {
            entity.HasKey(x => x.VolunteerId);

            entity.Property(x => x.Bio).IsRequired().HasMaxLength(2000);
            entity.Property(x => x.Experience).IsRequired().HasMaxLength(2000);

            entity.Property(x => x.MaxHoursPerWeek).IsRequired(false);

            // 1:1 User <-> VolunteerProfile (PK is FK)
            entity.HasOne(x => x.Volunteer)
                  .WithOne(u => u.VolunteerProfile)
                  .HasForeignKey<VolunteerProfile>(x => x.VolunteerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(x => x.IsApproved).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();

            entity.HasOne(x => x.Venue)
                  .WithMany() // Venue doesn't need navigation back
                  .HasForeignKey(x => x.VenueId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Index
            entity.HasIndex(x => x.PreferredVenueId);
            entity.HasIndex(x => x.VenueId);
            entity.HasIndex(x => x.IsApproved);
        }
    }
}

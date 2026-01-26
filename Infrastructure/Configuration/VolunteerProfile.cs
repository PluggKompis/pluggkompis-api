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

            entity.Property(x => x.Bio)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(x => x.Experience)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(x => x.MaxHoursPerWeek)
                .IsRequired(false);

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.UpdatedAt)
                .IsRequired(false);

            // 1:1 User <-> VolunteerProfile (PK is FK)
            entity.HasOne(x => x.Volunteer)
                .WithOne(u => u.VolunteerProfile)
                .HasForeignKey<VolunteerProfile>(x => x.VolunteerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional preferred venue (preference only)
            entity.HasOne(x => x.PreferredVenue)
                .WithMany()
                .HasForeignKey(x => x.PreferredVenueId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(x => x.PreferredVenueId);
        }
    }
}

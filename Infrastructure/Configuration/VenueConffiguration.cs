using Domain.Models.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class VenueConfiguration : IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> entity)
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Address).IsRequired().HasMaxLength(300);
            entity.Property(x => x.City).IsRequired().HasMaxLength(100);
            entity.Property(x => x.PostalCode).IsRequired().HasMaxLength(20);

            entity.Property(x => x.Description).IsRequired().HasMaxLength(2000);

            entity.Property(x => x.ContactEmail).IsRequired().HasMaxLength(256);
            entity.Property(x => x.ContactPhone).IsRequired().HasMaxLength(50);

            entity.Property(x => x.IsActive).IsRequired();

            // Venue -> TimeSlots
            entity.HasMany(x => x.TimeSlots)
                  .WithOne(ts => ts.Venue)
                  .HasForeignKey(ts => ts.VenueId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Venue <- VolunteerProfile.PreferredVenueId (optional)
            entity.HasMany(x => x.PreferredByVolunteers)
                  .WithOne(vp => vp.PreferredVenue)
                  .HasForeignKey(vp => vp.PreferredVenueId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(x => x.CoordinatorId);
            entity.HasIndex(x => x.City);
        }
    }
}

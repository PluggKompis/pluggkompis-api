using Domain.Models.Entities.Venues;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
    {
        public void Configure(EntityTypeBuilder<TimeSlot> entity)
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.DayOfWeek).IsRequired();
            entity.Property(x => x.StartTime).IsRequired();
            entity.Property(x => x.EndTime).IsRequired();

            entity.Property(x => x.MaxStudents).IsRequired();
            entity.Property(x => x.IsRecurring).IsRequired();
            entity.Property(x => x.SpecificDate).IsRequired(false);

            entity.Property(x => x.Status).IsRequired();

            entity.HasMany(x => x.Bookings)
                  .WithOne(b => b.TimeSlot)
                  .HasForeignKey(b => b.TimeSlotId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.VolunteerShifts)
                  .WithOne(vs => vs.TimeSlot)
                  .HasForeignKey(vs => vs.TimeSlotId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.Subjects)
                  .WithOne(tss => tss.TimeSlot)
                  .HasForeignKey(tss => tss.TimeSlotId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Index
            entity.HasIndex(x => x.VenueId);
            entity.HasIndex(x => new { x.VenueId, x.DayOfWeek });

            entity.HasIndex(x => x.Status);

            entity.HasIndex(x => new { x.VenueId, x.SpecificDate });
        }
    }
}

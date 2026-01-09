using Domain.Models.Entities.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> entity)
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.BookedAt).IsRequired();
            entity.Property(x => x.Status).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(2000).IsRequired(false);

            entity.Property(x => x.StudentId).IsRequired(false);
            entity.Property(x => x.ChildId).IsRequired(false);

            // index
            entity.HasIndex(x => x.TimeSlotId);
            entity.HasIndex(x => x.BookedByUserId);
            entity.HasIndex(x => new { x.TimeSlotId, x.StudentId });
            entity.HasIndex(x => new { x.TimeSlotId, x.ChildId });
            entity.HasIndex(x => x.Status);

            // Optional (SQL Server): enforce exactly one of StudentId/ChildId
            // entity.ToTable(t => t.HasCheckConstraint(
            //     "CK_Booking_StudentOrChild",
            //     "((StudentId IS NOT NULL AND ChildId IS NULL) OR (StudentId IS NULL AND ChildId IS NOT NULL))"
            // ));
        }
    }
}

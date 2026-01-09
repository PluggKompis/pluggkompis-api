using Domain.Models.Entities.Children;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class ChildConfiguration : IEntityTypeConfiguration<Child>
    {
        public void Configure(EntityTypeBuilder<Child> entity)
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.BirthYear).IsRequired();
            entity.Property(x => x.SchoolGrade).IsRequired().HasMaxLength(50);

            entity.HasMany(x => x.Bookings)
                  .WithOne(b => b.Child)
                  .HasForeignKey(b => b.ChildId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Index
            entity.HasIndex(x => x.ParentId);
        }
    }
}

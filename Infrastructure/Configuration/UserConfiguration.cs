using Domain.Models.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(x => x.LastName).IsRequired().HasMaxLength(100);

            entity.Property(x => x.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(x => x.Email).IsUnique();

            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.Role).IsRequired();

            entity.Property(x => x.CreatedAt).IsRequired();
            entity.Property(x => x.IsActive).IsRequired();

            // Coordinator -> Venues
            entity.HasMany(x => x.CoordinatedVenues)
                  .WithOne(v => v.Coordinator)
                  .HasForeignKey(v => v.CoordinatorId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Parent -> Children
            entity.HasMany(x => x.Children)
                  .WithOne(c => c.Parent)
                  .HasForeignKey(c => c.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);

            // BookedBy -> Bookings
            entity.HasMany(x => x.BookingsCreated)
                  .WithOne(b => b.BookedByUser)
                  .HasForeignKey(b => b.BookedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Student -> Bookings (optional FK)
            entity.HasMany(x => x.BookingsAsStudent)
                  .WithOne(b => b.Student)
                  .HasForeignKey(b => b.StudentId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Volunteer -> Shifts
            entity.HasMany(x => x.VolunteerShifts)
                  .WithOne(s => s.Volunteer)
                  .HasForeignKey(s => s.VolunteerId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Volunteer -> VolunteerSubjects join rows
            entity.HasMany(x => x.VolunteerSubjects)
                  .WithOne(vs => vs.Volunteer)
                  .HasForeignKey(vs => vs.VolunteerId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Domain.Models.Entities.Volunteers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class VolunteerApplicationConfiguration : IEntityTypeConfiguration<VolunteerApplication>
    {
        public void Configure(EntityTypeBuilder<VolunteerApplication> entity)
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(x => x.AppliedAt)
                .IsRequired();

            entity.Property(x => x.ReviewedAt)
                .IsRequired(false);

            entity.Property(x => x.DecisionNote)
                .HasMaxLength(1000)
                .IsRequired(false);

            // Volunteer (User) -> VolunteerApplications (1:M)
            entity.HasOne(x => x.Volunteer)
                .WithMany(u => u.VolunteerApplications)
                .HasForeignKey(x => x.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Venue (1:M)
            entity.HasOne(x => x.Venue)
                .WithMany(v => v.VolunteerApplications)
                .HasForeignKey(x => x.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            // ReviewedBy (User) -> ReviewedVolunteerApplications (1:M, optional)
            entity.HasOne(x => x.ReviewedByCoordinator)
                .WithMany(u => u.ReviewedVolunteerApplications)
                .HasForeignKey(x => x.ReviewedByCoordinatorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(x => new { x.VenueId, x.Status });
            entity.HasIndex(x => new { x.VolunteerId, x.Status });
            entity.HasIndex(x => x.AppliedAt);
        }

    }
}

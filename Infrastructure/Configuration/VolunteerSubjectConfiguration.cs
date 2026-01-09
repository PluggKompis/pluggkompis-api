using Domain.Models.Entities.JoinEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class VolunteerSubjectConfiguration : IEntityTypeConfiguration<VolunteerSubject>
    {
        public void Configure(EntityTypeBuilder<VolunteerSubject> entity)
        {
            entity.HasKey(x => new { x.VolunteerId, x.SubjectId });

            entity.Property(x => x.ConfidenceLevel).IsRequired();

            entity.HasOne(x => x.Volunteer)
                  .WithMany(u => u.VolunteerSubjects)
                  .HasForeignKey(x => x.VolunteerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Subject)
                  .WithMany(s => s.VolunteerSubjects)
                  .HasForeignKey(x => x.SubjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(x => x.SubjectId);

        }
    }
}

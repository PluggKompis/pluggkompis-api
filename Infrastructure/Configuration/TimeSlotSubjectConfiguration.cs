using Domain.Models.Entities.JoinEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class TimeSlotSubjectConfiguration : IEntityTypeConfiguration<TimeSlotSubject>
    {
        public void Configure(EntityTypeBuilder<TimeSlotSubject> entity)
        {
            entity.HasKey(x => new { x.TimeSlotId, x.SubjectId });

            entity.HasOne(x => x.TimeSlot)
                  .WithMany(ts => ts.Subjects)
                  .HasForeignKey(x => x.TimeSlotId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Subject)
                  .WithMany(s => s.TimeSlotSubjects)
                  .HasForeignKey(x => x.SubjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Indexes 
            entity.HasIndex(x => x.SubjectId);

        }
    }
}

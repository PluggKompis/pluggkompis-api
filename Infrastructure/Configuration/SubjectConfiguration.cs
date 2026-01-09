using Domain.Models.Entities.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> entity)
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Icon).HasMaxLength(100).IsRequired(false);

            entity.HasMany(x => x.TimeSlotSubjects)
                  .WithOne(tss => tss.Subject)
                  .HasForeignKey(tss => tss.SubjectId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.VolunteerSubjects)
                  .WithOne(vs => vs.Subject)
                  .HasForeignKey(vs => vs.SubjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Domain.Models;
using Domain.Models.Entities.Bookings;
using Domain.Models.Entities.Children;
using Domain.Models.Entities.JoinEntities;
using Domain.Models.Entities.Subjects;
using Domain.Models.Entities.Users;
using Domain.Models.Entities.Venues;
using Domain.Models.Entities.Volunteers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Venue> Venues => Set<Venue>();
        public DbSet<VolunteerProfile> VolunteerProfiles => Set<VolunteerProfile>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
        public DbSet<TimeSlotSubject> TimeSlotSubjects => Set<TimeSlotSubject>();
        public DbSet<Child> Children => Set<Child>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<VolunteerShift> VolunteerShifts => Set<VolunteerShift>();
        public DbSet<VolunteerSubject> VolunteerSubjects => Set<VolunteerSubject>();

        public DbSet<LogEntry> Logs => Set<LogEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

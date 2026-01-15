using Domain.Models.Entities.Subjects;
using Domain.Models.Entities.Users;
using Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // USERS
            if (!await context.Users.AnyAsync())
            {
                var coordinatorId = Guid.Parse("00000000-0000-0000-0000-000000000001");

                context.Users.Add(new User
                {
                    Id = coordinatorId,
                    FirstName = "Test",
                    LastName = "Coordinator",
                    Email = "coordinator@test.se",
                    PasswordHash = "DEV_ONLY_NO_AUTH",
                    Role = UserRole.Coordinator,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            // SUBJECTS
            if (!await context.Subjects.AnyAsync())
            {
                var subjects = new List<Subject>
        {
            new Subject { Id = Guid.NewGuid(), Name = "Matematik", Icon = "📐" },
            new Subject { Id = Guid.NewGuid(), Name = "Svenska", Icon = "📖" },
            new Subject { Id = Guid.NewGuid(), Name = "Engelska", Icon = "🇬🇧" },
            new Subject { Id = Guid.NewGuid(), Name = "Naturkunskap", Icon = "🌿" },
            new Subject { Id = Guid.NewGuid(), Name = "Fysik", Icon = "⚛️" },
            new Subject { Id = Guid.NewGuid(), Name = "Kemi", Icon = "🧪" },
            new Subject { Id = Guid.NewGuid(), Name = "Biologi", Icon = "🦠" },
            new Subject { Id = Guid.NewGuid(), Name = "Samhällskunskap", Icon = "🏛️" },
            new Subject { Id = Guid.NewGuid(), Name = "Historia", Icon = "📜" },
            new Subject { Id = Guid.NewGuid(), Name = "Geografi", Icon = "🌍" },
            new Subject { Id = Guid.NewGuid(), Name = "Idrott och hälsa", Icon = "⚽" },
            new Subject { Id = Guid.NewGuid(), Name = "Musik", Icon = "🎵" },
            new Subject { Id = Guid.NewGuid(), Name = "Bild", Icon = "🎨" },
            new Subject { Id = Guid.NewGuid(), Name = "Slöjd", Icon = "🔨" },
            new Subject { Id = Guid.NewGuid(), Name = "Teknik", Icon = "⚙️" },
            new Subject { Id = Guid.NewGuid(), Name = "Hem- och konsumentkunskap", Icon = "🍳" },
            new Subject { Id = Guid.NewGuid(), Name = "Programmering", Icon = "💻" },
            new Subject { Id = Guid.NewGuid(), Name = "Spanska", Icon = "🇪🇸" },
            new Subject { Id = Guid.NewGuid(), Name = "Franska", Icon = "🇫🇷" },
            new Subject { Id = Guid.NewGuid(), Name = "Tyska", Icon = "🇩🇪" },
        };

                await context.Subjects.AddRangeAsync(subjects);
            }

            await context.SaveChangesAsync();
        }
    }
}

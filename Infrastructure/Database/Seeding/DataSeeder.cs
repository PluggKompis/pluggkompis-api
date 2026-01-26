using Domain.Models.Entities.Subjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
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

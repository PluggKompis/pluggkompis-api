using Domain.Models.Entities.Users;
using Domain.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            //var faker = new Faker("en");

            // Prevent duplicate seed
            if (await context.Users.AnyAsync())
                return;

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

            await context.SaveChangesAsync();
        }


        //private static User CreateUserWithPassword(string username, string email, string password, Role[] roles)
        //{
        //    using var hmac = new HMACSHA512();
        //    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        //    var salt = hmac.Key;

        //    return new User
        //    {
        //        Id = Guid.NewGuid(),
        //        Username = username,
        //        Email = email,
        //        PasswordHash = hash,
        //        PasswordSalt = salt,
        //        Roles = roles.Select(role => new UserRole
        //        {
        //            RoleId = role.Id
        //        }).ToList()
        //    };
        //}
    }
}

using System.Security.Cryptography;
using System.Text;
using Bogus;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var faker = new Faker("en");

            // 1  Roles
            if (!await context.Roles.AnyAsync())
            {
                var roles = new[]
                {
            new Role { Name = "Admin" },
            new Role { Name = "User" },
            new Role { Name = "CompanyUser" },
            new Role { Name = "Auditor" },
            new Role { Name = "Manager" }
        };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            // 2️ Users
            if (!await context.Users.AnyAsync())
            {
                var roles = await context.Roles.ToListAsync();
                var adminRole = roles.First(r => r.Name == "Admin");
                var userRole = roles.First(r => r.Name == "User");
                var companyRole = roles.First(r => r.Name == "CompanyUser");

                var users = new List<User>();

                // Admin
                users.Add(CreateUserWithPassword("admin", "admin@bank.com", "admin123", new[] { adminRole }));

                // Regular users
                for (int i = 0; i < 5; i++)
                {
                    var user = CreateUserWithPassword(
                        faker.Internet.UserName(),
                        faker.Internet.Email(),
                        "user123",
                        new[] { userRole });
                    users.Add(user);
                }

                // Company/internal users
                for (int i = 0; i < 3; i++)
                {
                    var user = CreateUserWithPassword(
                        $"employee_{faker.Random.Number(100, 999)}",
                        faker.Internet.Email(),
                        "company123",
                        new[] { companyRole });
                    users.Add(user);
                }

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
        }


        private static User CreateUserWithPassword(string username, string email, string password, Role[] roles)
        {
            using var hmac = new HMACSHA512();
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var salt = hmac.Key;

            return new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Roles = roles.Select(role => new UserRole
                {
                    RoleId = role.Id
                }).ToList()
            };
        }
    }
}

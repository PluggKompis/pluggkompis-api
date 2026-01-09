using System.Security.Cryptography;
using System.Text;
using Bogus;
using Domain.Models.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var faker = new Faker("en");

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

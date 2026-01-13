using Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var jwtSecret = "ThisIsATestSecretKeyForJWTTokensMinimum32CharactersLong!!!";
            var jwtIssuer = "PluggKompis.Test";
            var jwtAudience = "PluggKompis.Test.Users";

            // Set configuration FIRST
            builder.UseConfiguration(new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JwtSettings:Secret"] = jwtSecret,
                    ["JwtSettings:Issuer"] = jwtIssuer,
                    ["JwtSettings:Audience"] = jwtAudience,
                    ["JwtSettings:ExpireMinutes"] = "60"
                })
                .Build());

            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Use a FIXED database name (not Guid) so all requests use the same DB
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");  // ✅ Fixed name
                });

                // Build service provider and ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            });
        }

    }
}

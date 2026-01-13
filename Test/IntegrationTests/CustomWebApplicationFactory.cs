//using API;
//using Infrastructure.Database;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.Data.Sqlite;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//namespace Test.IntegrationTests
//{
//    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
//    {
//        private SqliteConnection? _connection;

//        protected override IHost CreateHost(IHostBuilder builder)
//        {
//            //builder.ConfigureAppConfiguration(config =>
//            //{
//            //    var testConfig = new Dictionary<string, string?>
//            //    {
//            //        ["JwtSettings:Issuer"] = "TestIssuer",
//            //        ["JwtSettings:Audience"] = "TestAudience",
//            //        ["JwtSettings:Secret"] = "THIS_IS_A_TEST_SECRET_KEY_THAT_IS_AT_LEAST_32_CHARS",
//            //    };

//            //    config.AddInMemoryCollection(testConfig);
//            //});

//            builder.ConfigureAppConfiguration((context, config) =>
//            {
//                // Add test configuration
//                config.AddInMemoryCollection(new Dictionary<string, string?>
//                {
//                    ["JwtSettings:Secret"] = "YourTestSecretKeyThatIsAtLeast32CharactersLong!",
//                    ["JwtSettings:Issuer"] = "TestIssuer",
//                    ["JwtSettings:Audience"] = "TestAudience",
//                    ["JwtSettings:ExpirationInMinutes"] = "60"
//                });
//            });


//            builder.ConfigureServices(services =>
//            {
//                // Remove existing DbContext registration
//                var descriptor = services.SingleOrDefault(d =>
//                    d.ServiceType == typeof(DbContextOptions<AppDbContext>));

//                if (descriptor != null)
//                    services.Remove(descriptor);

//                _connection = new SqliteConnection("DataSource=:memory:");
//                _connection.Open();

//                services.AddDbContext<AppDbContext>(options =>
//                    options.UseSqlite(_connection));
//            });

//            var host = builder.Build();

//            // Ensure DB created AFTER host is built
//            using var scope = host.Services.CreateScope();
//            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//            db.Database.EnsureCreated();

//            host.Start();
//            return host;
//        }

//        protected override void Dispose(bool disposing)
//        {
//            base.Dispose(disposing);
//            _connection?.Dispose();
//        }
//    }
//}

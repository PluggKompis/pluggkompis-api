using Application.Common.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Database;
using Infrastructure.Database.Seeding;
using Infrastructure.Interceptors;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings")
            );

            services.AddScoped<IVenueRepository, VenueRepository>();
            services.AddSingleton<SaveChangesInterceptor, LogSaveChangesInterceptor>();

            // ✅ Declare connectionString BEFORE using it
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var interceptor = serviceProvider.GetRequiredService<SaveChangesInterceptor>();

                // ✅ Only use SQL Server if connection string exists
                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseSqlServer(connectionString);
                }
                // Otherwise, the test factory will configure in-memory database

                options.AddInterceptors(interceptor);
            });

            // Register repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // ✅ Only seed if using real SQL Server (not in-memory/tests)
            if (!string.IsNullOrEmpty(connectionString))
            {
                using var provider = services.BuildServiceProvider();
                using var scope = provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                DataSeeder.SeedAsync(db).GetAwaiter().GetResult();
            }

            // Auth services
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContextService, UserContextService>();

            return services;
        }
    }
}

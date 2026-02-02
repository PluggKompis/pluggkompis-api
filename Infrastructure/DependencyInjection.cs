using Application.Common.Interfaces;
using Infrastructure.Configuration;
using Infrastructure.Database;
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

            services.AddSingleton<SaveChangesInterceptor, LogSaveChangesInterceptor>();

            // Declare connectionString BEFORE using it
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                configuration.GetConnectionString("SQLAZURECONSTR_DefaultConnection");

            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var interceptor = serviceProvider.GetRequiredService<SaveChangesInterceptor>();
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        "Infrastructure misconfiguration: ConnectionStrings:DefaultConnection is missing");
                }

                options.UseSqlServer(connectionString);
                options.AddInterceptors(interceptor);
            });

            // Register repository
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IVenueRepository, VenueRepository>();
            services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVolunteerProfileRepository, VolunteerProfileRepository>();
            services.AddScoped<IVolunteerSubjectRepository, VolunteerSubjectRepository>();
            services.AddScoped<IVolunteerApplicationRepository, VolunteerApplicationRepository>();
            services.AddScoped<IVolunteerShiftRepository, VolunteerShiftRepository>();
            services.AddScoped<ICoordinatorDashboardRepository, CoordinatorDashboardRepository>();
            services.AddScoped<IAvailableShiftsRepository, AvailableShiftsRepository>();

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

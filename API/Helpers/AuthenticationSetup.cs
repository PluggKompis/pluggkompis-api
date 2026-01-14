using System.Text;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Helpers
{
    public static class AuthenticationSetup
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>();

            // Make it optional - don't throw if missing
            if (jwtSettings is null)
            {
                Console.WriteLine("⚠️ JWT settings not configured - skipping authentication setup");
                return services;
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
            {
                Console.WriteLine("⚠️ JWT Issuer not configured - skipping authentication setup");
                return services;
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
            {
                Console.WriteLine("⚠️ JWT Audience not configured - skipping authentication setup");
                return services;
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
            {
                Console.WriteLine("⚠️ JWT Secret not configured - skipping authentication setup");
                return services;
            }

            // Only configure if all settings are present
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)
                    )
                };
            });

            return services;
        }
    }
}

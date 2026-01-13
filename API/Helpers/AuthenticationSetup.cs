using System.Text;
using Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Helpers
{
    public static class AuthenticationSetup
    {
        //public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        //{
        //    var jwtSettings = configuration
        //        .GetSection("JwtSettings")
        //        .Get<JwtSettings>();

            if (jwtSettings is null)
                throw new InvalidOperationException("Missing configuration section 'JwtSettings'.");

            if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
                throw new InvalidOperationException("Missing configuration value 'JwtSettings:Issuer'.");

            if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
                throw new InvalidOperationException("Missing configuration value 'JwtSettings:Audience'.");

            if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
                throw new InvalidOperationException("Missing configuration value 'JwtSettings:Secret'.");

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        //    services.AddAuthentication(options =>
        //    {
        //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    })
        //    .AddJwtBearer(options =>
        //    {
        //        options.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = true,

        //            ValidIssuer = jwtSettings!.Issuer,
        //            ValidAudience = jwtSettings.Audience,
        //            IssuerSigningKey = new SymmetricSecurityKey(
        //                Encoding.UTF8.GetBytes(jwtSettings.Secret)
        //            )
        //        };
        //    });

        //    return services;
        //}
    }
}

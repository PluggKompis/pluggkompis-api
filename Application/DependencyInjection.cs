using Application.Common.Behaviors;
using Application.Volunteers.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
            services.AddAutoMapper(assembly);
            services.AddValidatorsFromAssembly(assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            // Business logic services
            services.AddScoped<IVolunteerService, VolunteerService>();
            services.AddScoped<IVolunteerProfileService, VolunteerProfileService>();

            return services;
        }
    }
}

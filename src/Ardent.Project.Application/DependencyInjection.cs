using Ardent.Project.Application.Common.Behaviours;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Ardent.Project.Application;

/// <summary>Registers the Application layer.</summary>
public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers MediatR handlers, pipeline behaviours and FluentValidation validators from this assembly.</summary>
        /// <param name="mediatRLicenseKey">MediatR licence key from configuration; null runs unlicensed (warnings only).</param>
        public IServiceCollection AddApplication(string? mediatRLicenseKey)
        {
            services.AddMediatR(config =>
            {
                config.LicenseKey = mediatRLicenseKey;
                config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

                // Logging is registered first so it wraps validation and times rejected requests too.
                config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
                config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}

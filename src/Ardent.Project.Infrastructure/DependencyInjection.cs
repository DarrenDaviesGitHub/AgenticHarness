using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Infrastructure.Persistence;
using Ardent.Project.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ardent.Project.Infrastructure;

/// <summary>Registers the Infrastructure layer.</summary>
public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers the database, repositories, the system clock and the database health check.</summary>
        public IServiceCollection AddInfrastructure()
        {
            services.AddOptions<DatabaseOptions>()
                .BindConfiguration(DatabaseOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddDbContext<ApplicationDbContext>((provider, options) =>
                options.UseSqlite(provider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString));

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddSingleton(TimeProvider.System);

            services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>("database");

            return services;
        }
    }
}

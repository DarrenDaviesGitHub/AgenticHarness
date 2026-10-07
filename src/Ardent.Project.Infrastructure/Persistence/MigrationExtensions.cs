using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ardent.Project.Infrastructure.Persistence;

/// <summary>Host helpers for database migrations.</summary>
public static class MigrationExtensions
{
    extension(IHost host)
    {
        /// <summary>Applies pending EF Core migrations. Call in Development only; production uses a migration bundle or script.</summary>
        public async Task ApplyMigrationsAsync(CancellationToken cancellationToken = default)
        {
            await using AsyncServiceScope scope = host.Services.CreateAsyncScope();
            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }
}

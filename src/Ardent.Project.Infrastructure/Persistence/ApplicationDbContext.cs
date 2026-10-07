using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Domain.Products;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Ardent.Project.Infrastructure.Persistence;

/// <summary>EF Core session for the application. Also the <see cref="IUnitOfWork"/>, since SaveChanges is already a unit of work.</summary>
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork
{
    // SQLITE_CONSTRAINT_UNIQUE: https://www.sqlite.org/rescode.html#constraint_unique
    private const int SqliteUniqueConstraintFailed = 2067;

    /// <summary>Products table.</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Saves changes for handlers. A unique-index violation (e.g. two requests racing for the same SKU)
    /// becomes <see cref="ConflictException"/>, so the API returns 409 instead of 500.
    /// </summary>
    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqliteException { SqliteExtendedErrorCode: SqliteUniqueConstraintFailed })
        {
            throw new ConflictException("The change conflicts with existing data: a unique value is already in use.", exception);
        }
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
}

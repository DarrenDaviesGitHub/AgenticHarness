using System.ComponentModel.DataAnnotations;

namespace Ardent.Project.Infrastructure.Persistence;

/// <summary>Database settings bound from the <c>Database</c> configuration section and validated at startup.</summary>
public sealed class DatabaseOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Database";

    /// <summary>SQLite connection string, e.g. <c>Data Source=app.db</c>.</summary>
    [Required]
    public string ConnectionString { get; init; } = string.Empty;
}

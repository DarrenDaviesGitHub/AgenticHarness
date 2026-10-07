namespace Ardent.Project.Application.Common.Interfaces;

/// <summary>Commits all staged repository changes in one transaction.</summary>
public interface IUnitOfWork
{
    /// <summary>Saves all staged changes and returns the number of affected rows.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

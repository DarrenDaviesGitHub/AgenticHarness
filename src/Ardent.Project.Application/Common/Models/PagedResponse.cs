namespace Ardent.Project.Application.Common.Models;

/// <summary>One page of results plus the information a client needs to request the next one.</summary>
/// <param name="Items">The items on this page.</param>
/// <param name="Page">1-based page number.</param>
/// <param name="PageSize">Maximum number of items per page.</param>
/// <param name="TotalCount">Total number of items across all pages.</param>
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    /// <summary>Total number of pages.</summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

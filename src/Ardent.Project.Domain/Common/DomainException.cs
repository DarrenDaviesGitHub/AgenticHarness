namespace Ardent.Project.Domain.Common;

/// <summary>
/// Thrown when an operation would break a domain invariant. The API maps it to 400 Bad Request.
/// </summary>
public sealed class DomainException : Exception
{
    /// <summary>Creates the exception with no message.</summary>
    public DomainException()
    {
    }

    /// <summary>Creates the exception with the rule that was broken.</summary>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <summary>Creates the exception with the rule that was broken and the underlying cause.</summary>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

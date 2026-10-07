namespace Ardent.Project.Application.Common.Exceptions;

/// <summary>Thrown when a request conflicts with the current state, e.g. a duplicate SKU. The API maps it to 409 Conflict.</summary>
public sealed class ConflictException : Exception
{
    /// <summary>Creates the exception with no message.</summary>
    public ConflictException()
    {
    }

    /// <summary>Creates the exception describing the conflict.</summary>
    public ConflictException(string message)
        : base(message)
    {
    }

    /// <summary>Creates the exception describing the conflict and the underlying cause.</summary>
    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

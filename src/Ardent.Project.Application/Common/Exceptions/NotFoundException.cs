namespace Ardent.Project.Application.Common.Exceptions;

/// <summary>Thrown when a requested resource does not exist. The API maps it to 404 Not Found.</summary>
public sealed class NotFoundException : Exception
{
    /// <summary>Creates the exception with no message.</summary>
    public NotFoundException()
    {
    }

    /// <summary>Creates the exception with a custom message.</summary>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>Creates the exception with a custom message and the underlying cause.</summary>
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Creates the exception for a resource name and key, for example "Product '42' was not found."</summary>
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} '{key}' was not found.")
    {
    }
}

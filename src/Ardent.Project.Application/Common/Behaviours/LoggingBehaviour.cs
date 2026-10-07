using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ardent.Project.Application.Common.Behaviours;

/// <summary>Logs the name of each request and how long it took.</summary>
public sealed class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        long started = Stopwatch.GetTimestamp();

        try
        {
            TResponse response = await next(cancellationToken);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMilliseconds:0.0} ms",
                    requestName,
                    Stopwatch.GetElapsedTime(started).TotalMilliseconds);
            }

            return response;
        }
        catch (Exception exception) when (logger.IsEnabled(LogLevel.Warning))
        {
            // Expected failures (validation, not found) are warnings; GlobalExceptionHandler logs real 500s as errors.
            logger.LogWarning(
                "Failed {RequestName} in {ElapsedMilliseconds:0.0} ms with {ExceptionType}",
                requestName,
                Stopwatch.GetElapsedTime(started).TotalMilliseconds,
                exception.GetType().Name);
            throw;
        }
    }
}

using System.Text.Json;
using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Ardent.Project.Api.ExceptionHandling;

/// <summary>Maps every exception to an RFC 9457 ProblemDetails response (see docs/adr/0003).</summary>
internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // The client went away: nobody reads the response, and it is not a server error.
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            return true;
        }

        ProblemDetails problem = exception switch
        {
            ValidationException validation => new ValidationProblemDetails(ToErrors(validation))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred.",
            },
            DomainException => Problem(StatusCodes.Status400BadRequest, "A business rule was violated.", exception.Message),
            NotFoundException => Problem(StatusCodes.Status404NotFound, "The resource was not found.", exception.Message),
            ConflictException => Problem(StatusCodes.Status409Conflict, "The request conflicts with the current state.", exception.Message),
            _ => Unexpected(exception),
        };

        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception,
        });
    }

    private static ProblemDetails Problem(int status, string title, string detail) =>
        new() { Status = status, Title = title, Detail = detail };

    private static Dictionary<string, string[]> ToErrors(ValidationException exception) =>
        exception.Errors
            // camelCase keys match the JSON contract and the [ApiController] binding errors.
            .GroupBy(failure => JsonNamingPolicy.CamelCase.ConvertName(failure.PropertyName))
            .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorMessage).Distinct().ToArray());

    private ProblemDetails Unexpected(Exception exception)
    {
        logger.LogError(exception, "Unhandled exception");

        // Never leak internals outside Development.
        return Problem(
            StatusCodes.Status500InternalServerError,
            "An unexpected error occurred.",
            environment.IsDevelopment() ? exception.Message : "Quote the traceId when reporting this error.");
    }
}

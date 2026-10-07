using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ardent.Project.Application.Common.Behaviours;

/// <summary>Runs every FluentValidation validator for the request and throws one <see cref="ValidationException"/> with all failures.</summary>
public sealed class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationContext<TRequest> context = new(request);
        ValidationResult[] results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        List<ValidationFailure> failures = [.. results.SelectMany(r => r.Errors)];

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}

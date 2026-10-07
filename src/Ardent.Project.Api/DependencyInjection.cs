using System.Diagnostics;
using Ardent.Project.Api.ExceptionHandling;
using Microsoft.OpenApi;

namespace Ardent.Project.Api;

/// <summary>Registers the HTTP layer: controllers, ProblemDetails, exception handling and OpenAPI.</summary>
internal static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPresentation()
        {
            // Keep the Async suffix in action names so CreatedAtAction(nameof(GetByIdAsync)) resolves.
            services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);

            services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions.TryAdd(
                    "traceId",
                    Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
            });
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddOpenApi(options => options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Ardent.Project API",
                    Version = "v1",
                    Description = "Products sample built on Clean Architecture, CQRS (MediatR) and RFC 9457 ProblemDetails.",
                };
                return Task.CompletedTask;
            }));

            return services;
        }
    }
}

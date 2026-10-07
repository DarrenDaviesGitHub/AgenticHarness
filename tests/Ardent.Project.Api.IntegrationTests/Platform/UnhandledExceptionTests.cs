using Ardent.Project.Api.IntegrationTests.Infrastructure;
using Ardent.Project.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Ardent.Project.Api.IntegrationTests.Platform;

public sealed class UnhandledExceptionTests(UnhandledExceptionTests.FailingProductionApiFactory factory)
    : IClassFixture<UnhandledExceptionTests.FailingProductionApiFactory>
{
    private const string SecretMessage = "connection string with a password";

    [Fact]
    public async Task AnyEndpoint_UnexpectedExceptionInProduction_Returns500WithoutInternals()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync($"{ApiClient.ProductsUrl}/{ApiClient.UnknownId}", TestContext.Current.CancellationToken);

        ProblemDetails problem = await response.ShouldBeProblemAsync<ProblemDetails>(500);
        problem.Detail.ShouldNotBeNull().ShouldNotContain(SecretMessage);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldNotContain("   at ");
    }

    /// <summary>Production environment with a repository that always throws, to exercise the 500 path.</summary>
    public sealed class FailingProductionApiFactory : ApiFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Production");
            builder.ConfigureTestServices(services =>
            {
                IProductRepository repository = Substitute.For<IProductRepository>();
                repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                    .Returns<Task<Domain.Products.Product?>>(_ => throw new InvalidOperationException(SecretMessage));
                services.AddScoped(_ => repository);
            });
        }
    }
}

using Ardent.Project.Api.IntegrationTests.Infrastructure;
using Ardent.Project.Application.Common.Exceptions;
using Ardent.Project.Application.Common.Interfaces;
using Ardent.Project.Domain.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Ardent.Project.Api.IntegrationTests.Persistence;

public sealed class UnitOfWorkTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task SaveChangesAsync_DuplicateSkuThatBypassedThePreCheck_ThrowsConflictException()
    {
        _ = factory.CreateClient(); // Starts the host, which applies migrations.
        await using AsyncServiceScope scope = factory.Services.CreateAsyncScope();
        IProductRepository products = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        string sku = ApiClient.UniqueSku();
        DateTimeOffset now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        products.Add(Product.Create("First", null, sku, 1m, now));
        products.Add(Product.Create("Second", null, sku, 2m, now));

        await Should.ThrowAsync<ConflictException>(() => unitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken));
    }
}

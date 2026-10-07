using System.Net.Http.Json;
using Ardent.Project.Api.Contracts;
using Ardent.Project.Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace Ardent.Project.Api.IntegrationTests.Infrastructure;

/// <summary>Small helpers that keep the tests focused on the behaviour under test.</summary>
internal static class ApiClient
{
    public const string ProductsUrl = "/api/products";

    public static readonly Guid UnknownId = Guid.Parse("0199a1b2-0000-7000-8000-000000000000");

    public static CreateProductRequest NewProduct(string? sku = null, decimal price = 129.99m) =>
        new("Mechanical keyboard", "Tenkeyless", sku ?? UniqueSku(), price);

    public static string UniqueSku() => $"SKU-{Guid.NewGuid():N}"[..20];

    public static async Task<ProductResponse> CreateProductAsync(this HttpClient client, CreateProductRequest request)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(ProductsUrl, request, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken))!;
    }

    /// <summary>Asserts the response is RFC 9457 ProblemDetails with traceId and instance, and returns it.</summary>
    public static async Task<TProblem> ShouldBeProblemAsync<TProblem>(this HttpResponseMessage response, int expectedStatus)
        where TProblem : ProblemDetails
    {
        ((int)response.StatusCode).ShouldBe(expectedStatus);
        response.Content.Headers.ContentType.ShouldNotBeNull().MediaType.ShouldBe("application/problem+json");

        TProblem problem = (await response.Content.ReadFromJsonAsync<TProblem>(TestContext.Current.CancellationToken))!;
        problem.Status.ShouldBe(expectedStatus);
        problem.Instance.ShouldNotBeNullOrWhiteSpace();
        problem.Extensions.ShouldContainKey("traceId");
        return problem;
    }
}

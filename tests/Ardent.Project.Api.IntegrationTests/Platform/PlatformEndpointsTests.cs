using System.Net;
using System.Text.Json;
using Ardent.Project.Api.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Ardent.Project.Api.IntegrationTests.Platform;

public sealed class PlatformEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Health_DatabaseReachable_Returns200Healthy()
    {
        HttpResponseMessage response = await _client.GetAsync("/health", CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync(CancellationToken)).ShouldBe("Healthy");
    }

    [Fact]
    public async Task OpenApiDocument_Development_Returns31DocumentWithProductRoutes()
    {
        string json = await _client.GetStringAsync("/openapi/v1.json", CancellationToken);

        using JsonDocument document = JsonDocument.Parse(json);
        document.RootElement.GetProperty("openapi").GetString().ShouldStartWith("3.1");
        JsonElement paths = document.RootElement.GetProperty("paths");
        paths.TryGetProperty("/api/products", out _).ShouldBeTrue();
        paths.TryGetProperty("/api/products/{id}", out _).ShouldBeTrue();
    }

    [Fact]
    public async Task ScalarUi_Development_Returns200Html()
    {
        HttpResponseMessage response = await _client.GetAsync("/scalar", CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("text/html");
    }

    [Fact]
    public async Task UnknownRoute_Returns404Problem()
    {
        HttpResponseMessage response = await _client.GetAsync("/does-not-exist", CancellationToken);

        await response.ShouldBeProblemAsync<ProblemDetails>(404);
    }
}

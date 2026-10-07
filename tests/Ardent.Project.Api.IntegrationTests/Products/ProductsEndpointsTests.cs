using System.Net;
using System.Net.Http.Json;
using Ardent.Project.Api.Contracts;
using Ardent.Project.Api.IntegrationTests.Infrastructure;
using Ardent.Project.Application.Common.Models;
using Ardent.Project.Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace Ardent.Project.Api.IntegrationTests.Products;

/// <summary>One test per row of the HTTP contract table in CLAUDE.md, plus the error shapes.</summary>
public sealed class ProductsEndpointsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task List_DefaultPaging_Returns200WithPage()
    {
        ProductResponse created = await _client.CreateProductAsync(ApiClient.NewProduct());

        PagedResponse<ProductResponse>? page = await _client.GetFromJsonAsync<PagedResponse<ProductResponse>>(
            ApiClient.ProductsUrl, CancellationToken);

        page.ShouldNotBeNull();
        page.Page.ShouldBe(1);
        page.PageSize.ShouldBe(20);
        page.Items.ShouldContain(p => p.Id == created.Id);
    }

    [Fact]
    public async Task List_InvalidPaging_Returns400ValidationProblem()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ApiClient.ProductsUrl}?page=0&pageSize=500", CancellationToken);

        ValidationProblemDetails problem = await response.ShouldBeProblemAsync<ValidationProblemDetails>(400);
        problem.Errors.Keys.ShouldBe(["page", "pageSize"], ignoreOrder: true);
    }

    [Fact]
    public async Task GetById_ExistingProduct_Returns200WithProduct()
    {
        ProductResponse created = await _client.CreateProductAsync(ApiClient.NewProduct(price: 42m));

        ProductResponse? fetched = await _client.GetFromJsonAsync<ProductResponse>(
            $"{ApiClient.ProductsUrl}/{created.Id}", CancellationToken);

        fetched.ShouldBe(created);
    }

    [Fact]
    public async Task GetById_UnknownId_Returns404Problem()
    {
        HttpResponseMessage response = await _client.GetAsync($"{ApiClient.ProductsUrl}/{ApiClient.UnknownId}", CancellationToken);

        ProblemDetails problem = await response.ShouldBeProblemAsync<ProblemDetails>(404);
        problem.Detail.ShouldNotBeNull().ShouldContain(ApiClient.UnknownId.ToString());
    }

    [Fact]
    public async Task Create_ValidRequest_Returns201WithLocationAndBody()
    {
        CreateProductRequest request = ApiClient.NewProduct(sku: "new-sku-1");

        HttpResponseMessage response = await _client.PostAsJsonAsync(ApiClient.ProductsUrl, request, CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        ProductResponse body = (await response.Content.ReadFromJsonAsync<ProductResponse>(CancellationToken))!;
        body.Sku.ShouldBe("NEW-SKU-1");
        response.Headers.Location!.AbsolutePath.ShouldBe($"{ApiClient.ProductsUrl}/{body.Id}");
    }

    [Fact]
    public async Task Create_InvalidRequest_Returns400ValidationProblemPerField()
    {
        CreateProductRequest request = new(Name: "", Description: null, Sku: "not valid!", Price: 0);

        HttpResponseMessage response = await _client.PostAsJsonAsync(ApiClient.ProductsUrl, request, CancellationToken);

        ValidationProblemDetails problem = await response.ShouldBeProblemAsync<ValidationProblemDetails>(400);
        problem.Errors.Keys.ShouldBe(["name", "sku", "price"], ignoreOrder: true);
    }

    [Fact]
    public async Task Create_MalformedJson_Returns400ValidationProblem()
    {
        using StringContent body = new("{\"name\":", System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PostAsync(ApiClient.ProductsUrl, body, CancellationToken);

        await response.ShouldBeProblemAsync<ValidationProblemDetails>(400);
    }

    [Fact]
    public async Task Create_DuplicateSku_Returns409Problem()
    {
        ProductResponse existing = await _client.CreateProductAsync(ApiClient.NewProduct());

        HttpResponseMessage response = await _client.PostAsJsonAsync(
            ApiClient.ProductsUrl, ApiClient.NewProduct(sku: existing.Sku.ToLowerInvariant()), CancellationToken);

        ProblemDetails problem = await response.ShouldBeProblemAsync<ProblemDetails>(409);
        problem.Detail.ShouldNotBeNull().ShouldContain(existing.Sku);
    }

    [Fact]
    public async Task Update_ValidRequest_Returns204AndPersistsChanges()
    {
        ProductResponse created = await _client.CreateProductAsync(ApiClient.NewProduct());
        UpdateProductRequest request = new("Renamed", "Updated", created.Sku, 99.5m);

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{ApiClient.ProductsUrl}/{created.Id}", request, CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        ProductResponse? updated = await _client.GetFromJsonAsync<ProductResponse>($"{ApiClient.ProductsUrl}/{created.Id}", CancellationToken);
        updated!.Name.ShouldBe("Renamed");
        updated.Price.ShouldBe(99.5m);
        updated.UpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task Update_InvalidRequest_Returns400ValidationProblem()
    {
        ProductResponse created = await _client.CreateProductAsync(ApiClient.NewProduct());
        UpdateProductRequest request = new("", null, created.Sku, -1m);

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{ApiClient.ProductsUrl}/{created.Id}", request, CancellationToken);

        ValidationProblemDetails problem = await response.ShouldBeProblemAsync<ValidationProblemDetails>(400);
        problem.Errors.Keys.ShouldBe(["name", "price"], ignoreOrder: true);
    }

    [Fact]
    public async Task Update_UnknownId_Returns404Problem()
    {
        UpdateProductRequest request = new("Ghost", null, ApiClient.UniqueSku(), 1m);

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{ApiClient.ProductsUrl}/{ApiClient.UnknownId}", request, CancellationToken);

        await response.ShouldBeProblemAsync<ProblemDetails>(404);
    }

    [Fact]
    public async Task Update_SkuOfAnotherProduct_Returns409Problem()
    {
        ProductResponse first = await _client.CreateProductAsync(ApiClient.NewProduct());
        ProductResponse second = await _client.CreateProductAsync(ApiClient.NewProduct());
        UpdateProductRequest request = new(second.Name, second.Description, first.Sku, second.Price);

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{ApiClient.ProductsUrl}/{second.Id}", request, CancellationToken);

        await response.ShouldBeProblemAsync<ProblemDetails>(409);
    }

    [Fact]
    public async Task Delete_ExistingProduct_Returns204AndRemovesIt()
    {
        ProductResponse created = await _client.CreateProductAsync(ApiClient.NewProduct());

        HttpResponseMessage response = await _client.DeleteAsync($"{ApiClient.ProductsUrl}/{created.Id}", CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        HttpResponseMessage fetch = await _client.GetAsync($"{ApiClient.ProductsUrl}/{created.Id}", CancellationToken);
        fetch.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_UnknownId_Returns404Problem()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{ApiClient.ProductsUrl}/{ApiClient.UnknownId}", CancellationToken);

        await response.ShouldBeProblemAsync<ProblemDetails>(404);
    }
}

using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WarehouseInventory.Api.Contracts.Products;
using WarehouseInventory.Api.Infrastructure.Data;
using WarehouseInventory.Api.Tests.Fixtures;

namespace WarehouseInventory.Api.Tests.Controllers;

public class ProductsControllerTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture fixture;

    public ProductsControllerTests(ApiFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task Create_Persists_Product_And_Returns_Created()
    {
        await fixture.ResetDatabaseAsync();

        var sku = $"SKU-{Guid.NewGuid():N}";
        var barcode = $"BAR-{Guid.NewGuid():N}";
        var request = new CreateProductRequest(
            sku,
            barcode,
            "Test product",
            "ELE",
            "MRB",
            99.95m,
            10);

        var response = await fixture.Client.PostAsJsonAsync("/api/products", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProductDetailsDto>();
        Assert.NotNull(body);
        Assert.Equal(sku, body!.Sku);
        Assert.Equal(barcode, body.Barcode);
        Assert.Equal("Test product", body.Name);
        Assert.Equal("ELE", body.Category);
        Assert.Equal("MRB", body.Location);
        Assert.Equal(99.95m, body.Price);
        Assert.Equal(10, body.ReorderThreshold);
        Assert.True(body.IsActive);

        using var scope = fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        var product = await dbContext.Products
            .Include(item => item.Category)
            .Include(item => item.Location)
            .SingleAsync(item => item.Id == body.Id);

        Assert.Equal(sku, product.Sku);
        Assert.Equal(barcode, product.Barcode);
        Assert.Equal("Test product", product.Name);
        Assert.Equal("ELE", product.CategoryId);
        Assert.Equal("MRB", product.LocationId);
        Assert.Equal(99.95m, product.Price);
        Assert.Equal(10, product.ReorderThreshold);
        Assert.True(product.IsActive);

        var createdCategory = await dbContext.Categories.SingleAsync(item => item.Id == "ELE");
        var createdLocation = await dbContext.WarehouseLocations.SingleAsync(item => item.Id == "MRB");

        Assert.Equal("ELE", createdCategory.Id);
        Assert.Equal("MRB", createdLocation.Id);
    }

    [Fact]
    public async Task Create_Returns_Conflict_When_Sku_Already_Exists()
    {
        await fixture.ResetDatabaseAsync();

        var sku = $"SKU-{Guid.NewGuid():N}";
        var firstBarcode = $"BAR-{Guid.NewGuid():N}";
        var secondBarcode = $"BAR-{Guid.NewGuid():N}";

        await CreateProductAsync(sku, firstBarcode);

        var response = await fixture.Client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest(
                sku,
                secondBarcode,
                "Duplicate sku",
                "ELE",
                "MRB",
                50m,
                3));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(body);
        Assert.Equal("SKU already exists.", body!["message"]);
    }

    [Fact]
    public async Task Create_Returns_Conflict_When_Barcode_Already_Exists()
    {
        await fixture.ResetDatabaseAsync();

        var firstSku = $"SKU-{Guid.NewGuid():N}";
        var barcode = $"BAR-{Guid.NewGuid():N}";
        var secondSku = $"SKU-{Guid.NewGuid():N}";

        await CreateProductAsync(firstSku, barcode);

        var response = await fixture.Client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest(
                secondSku,
                barcode,
                "Duplicate barcode",
                "ELE",
                "MRB",
                60m,
                4));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(body);
        Assert.Equal("Barcode already exists.", body!["message"]);
    }

    private async Task CreateProductAsync(string sku, string? barcode)
    {
        var response = await fixture.Client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest(
                sku,
                barcode,
                "Seed product",
                "ELE",
                "MRB",
                25m,
                2));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
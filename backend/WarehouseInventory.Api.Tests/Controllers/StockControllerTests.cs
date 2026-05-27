using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WarehouseInventory.Api.Contracts.Stock;
using WarehouseInventory.Api.Domain.Entities;
using WarehouseInventory.Api.Infrastructure.Data;
using WarehouseInventory.Api.Tests.Fixtures;

namespace WarehouseInventory.Api.Tests.Controllers;

public class StockControllerTests : IClassFixture<ApiFixture>
{
    private readonly ApiFixture fixture;

    public StockControllerTests(ApiFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task SaveMovement_Guards_And_Persists_Product_Stock_And_Movement()
    {
        await fixture.ResetDatabaseAsync();
        var seed = await SeedProductAsync(currentStock: 10);

        var request = new StockMovementRequest(
            seed.ProductId,
            5,
            "adjustment",
            "tester",
            seed.Version);

        var response = await fixture.Client.PostAsJsonAsync("/api/stock/movements", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<StockMovementResultDto>();
        Assert.NotNull(body);
        Assert.Equal(seed.ProductId, body!.ProductId);
        Assert.Equal(10, body.PreviousStock);
        Assert.Equal(15, body.NewStock);
        Assert.Equal(seed.ProductId, body.Movement.ProductId);
        Assert.Equal(5, body.Movement.QuantityChange);
        Assert.Equal(10, body.Movement.PreviousStock);
        Assert.Equal(15, body.Movement.NewStock);
        Assert.Equal("adjustment", body.Movement.Reason);
        Assert.Equal("tester", body.Movement.CreatedBy);

        using var scope = fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        var product = await dbContext.Products.SingleAsync(item => item.Id == seed.ProductId);
        var movement = await dbContext.StockMovements.SingleAsync(item => item.ProductId == seed.ProductId);

        Assert.Equal(15, product.CurrentStock);
        Assert.Equal(10, movement.PreviousStock);
        Assert.Equal(15, movement.NewStock);
        Assert.Equal("adjustment", movement.Reason);
        Assert.Equal("tester", movement.CreatedBy);
    }

    [Fact]
    public async Task SaveMovement_Returns_BadRequest_For_Insufficient_Stock_And_Does_Not_Persist_Changes()
    {
        await fixture.ResetDatabaseAsync();
        var seed = await SeedProductAsync(currentStock: 3);

        var request = new StockMovementRequest(
            seed.ProductId,
            5,
            "shipped",
            "tester",
            seed.Version);

        var response = await fixture.Client.PostAsJsonAsync("/api/stock/movements", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(body);
        Assert.Equal("Insufficient stock", body!["message"]);

        using var scope = fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        var product = await dbContext.Products.SingleAsync(item => item.Id == seed.ProductId);
        var movementCount = await dbContext.StockMovements.CountAsync(item => item.ProductId == seed.ProductId);

        Assert.Equal(3, product.CurrentStock);
        Assert.Equal(0, movementCount);
    }

    [Fact]
    public async Task SaveMovement_Returns_Conflict_When_Version_Is_Stale_And_Persisted_State_Does_Not_Change()
    {
        await fixture.ResetDatabaseAsync();
        var seed = await SeedProductAsync(currentStock: 10);

        using (var scope = fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
            var product = await dbContext.Products.SingleAsync(item => item.Id == seed.ProductId);
            product.Version = Guid.NewGuid();
            await dbContext.SaveChangesAsync();
        }

        var request = new StockMovementRequest(
            seed.ProductId,
            2,
            "adjustment",
            "tester",
            seed.Version);

        var response = await fixture.Client.PostAsJsonAsync("/api/stock/movements", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(body);
        Assert.Equal("Stock was modified by another user. Refresh and retry.", body!["message"]);

        using var verifyScope = fixture.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        var productAfter = await verifyDb.Products.SingleAsync(item => item.Id == seed.ProductId);
        var movementCount = await verifyDb.StockMovements.CountAsync(item => item.ProductId == seed.ProductId);

        Assert.Equal(10, productAfter.CurrentStock);
        Assert.Equal(0, movementCount);
    }

    private async Task<ProductSeed> SeedProductAsync(int currentStock)
    {
        using var scope = fixture.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        var categoryId = CreateId("CAT");
        var locationId = CreateId("LOC");
        var productId = Guid.NewGuid();
        var version = Guid.NewGuid();

        dbContext.Categories.Add(new Category { Id = categoryId, Name = categoryId });
        dbContext.WarehouseLocations.Add(new WarehouseLocation { Id = locationId, Name = locationId });
        dbContext.Products.Add(new Product
        {
            Id = productId,
            Sku = $"SKU-{Guid.NewGuid():N}",
            Barcode = $"BAR-{Guid.NewGuid():N}",
            Name = "Test product",
            CategoryId = categoryId,
            LocationId = locationId,
            Price = 10m,
            CurrentStock = currentStock,
            ReorderThreshold = 2,
            IsActive = true,
            Version = version
        });

        await dbContext.SaveChangesAsync();

        return new ProductSeed(productId, version);
    }

    private static string CreateId(string prefix) => $"{prefix}-{Guid.NewGuid():N}"[..16];

    private sealed record ProductSeed(Guid ProductId, Guid Version);
}
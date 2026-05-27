using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WarehouseInventory.Api.Infrastructure.Data;

namespace WarehouseInventory.Api.Tests.Fixtures;

public sealed class ApiFixture : IAsyncLifetime
{
    private SqliteConnection connection = null!;
    private WebApplicationFactory<Program> factory = null!;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(service =>
                        service.ServiceType == typeof(DbContextOptions<WarehouseDbContext>));

                    if (descriptor is not null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<WarehouseDbContext>(options =>
                        options.UseSqlite(connection));
                });
            });

        Client = factory.CreateClient();

        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await factory.DisposeAsync();
        await connection.DisposeAsync();
    }

    public IServiceScope CreateScope() => factory.Services.CreateScope();

    public async Task ResetDatabaseAsync()
    {
        using var scope = CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }
}
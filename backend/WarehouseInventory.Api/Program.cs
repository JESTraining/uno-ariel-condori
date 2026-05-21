using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using WarehouseInventory.Api.Infrastructure.Data;
using WarehouseInventory.Api.Infrastructure.Repositories;
using WarehouseInventory.Api.Mapping;
using WarehouseInventory.Api.Services;
using WarehouseInventory.Api.Validation;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings__DefaultConnection"]
    ?? throw new InvalidOperationException("DefaultConnection is not configured.");

builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Register all AutoMapper profiles from the Mapping namespace
builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation(options =>
    options.DisableDataAnnotationsValidation = true);
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Configuration.GetValue("ApplyMigrations", false))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

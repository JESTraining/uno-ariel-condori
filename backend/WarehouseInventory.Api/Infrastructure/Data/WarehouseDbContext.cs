using System;
using Microsoft.EntityFrameworkCore;
using WarehouseInventory.Api.Domain.Entities;

namespace WarehouseInventory.Api.Infrastructure.Data;

public class WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<WarehouseLocation> WarehouseLocations => Set<WarehouseLocation>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Id).HasMaxLength(16).IsRequired();
            entity.Property(category => category.Name).HasColumnType("citext").HasMaxLength(120).IsRequired();
            entity.HasIndex(category => category.Name).IsUnique();

            entity.HasData(
                new Category { Id = "ELE", Name = "Electronics" },
                new Category { Id = "OFS", Name = "Office Supplies" },
                new Category { Id = "FUR", Name = "Furniture" },
                new Category { Id = "CLE", Name = "Cleaning" });
        });

        modelBuilder.Entity<WarehouseLocation>(entity =>
        {
            entity.HasKey(location => location.Id);
            entity.Property(location => location.Id).HasMaxLength(16).IsRequired();
            entity.Property(location => location.Name).HasMaxLength(120).IsRequired();
            entity.HasIndex(location => location.Name).IsUnique();

            entity.HasData(
                new WarehouseLocation { Id = "MRB", Name = "Main receiving bay" },
                new WarehouseLocation { Id = "BUS", Name = "Bulk storage" },
                new WarehouseLocation { Id = "PIA", Name = "Picking aisle" },
                new WarehouseLocation { Id = "OUT", Name = "Outbound staging" });
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Sku).HasColumnType("citext").HasMaxLength(64).IsRequired();
            entity.Property(product => product.Barcode).HasMaxLength(128);
            entity.Property(product => product.Name).HasColumnType("citext").HasMaxLength(200).IsRequired();
            entity.Property(product => product.CategoryId).HasMaxLength(16).IsRequired();
            entity.Property(product => product.LocationId).HasMaxLength(16).IsRequired();
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.Property(product => product.Version).IsConcurrencyToken();
            entity.HasIndex(product => product.Sku).IsUnique();
            entity.HasIndex(product => product.Barcode).IsUnique().HasFilter("\"Barcode\" IS NOT NULL");
            entity.HasIndex(product => new { product.CategoryId, product.IsActive });
            entity.HasIndex(product => new { product.LocationId, product.IsActive });

            entity.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(product => product.Location)
                .WithMany(location => location.Products)
                .HasForeignKey(product => product.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(movement => movement.Id);
            entity.Property(movement => movement.Reason).HasMaxLength(500).IsRequired();
            entity.Property(movement => movement.CreatedBy).HasMaxLength(120).IsRequired();
            entity.HasIndex(movement => new { movement.ProductId, movement.CreatedAt });

            entity.HasOne(movement => movement.Product)
                .WithMany(product => product.StockMovements)
                .HasForeignKey(movement => movement.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

namespace WarehouseInventory.Api.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Sku { get; set; }
    public string? Barcode { get; set; }
    public required string Name { get; set; }
    public Guid CategoryId { get; set; }
    public Guid LocationId { get; set; }
    public decimal Price { get; set; }
    public int CurrentStock { get; set; }
    public int ReorderThreshold { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid Version { get; set; } = Guid.NewGuid();

    public Category? Category { get; set; }
    public WarehouseLocation? Location { get; set; }
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}

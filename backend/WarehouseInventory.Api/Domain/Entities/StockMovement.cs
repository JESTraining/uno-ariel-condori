namespace WarehouseInventory.Api.Domain.Entities;

public class StockMovement
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public int QuantityChange { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public required string Reason { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public required string CreatedBy { get; set; }

    public Product? Product { get; set; }
}

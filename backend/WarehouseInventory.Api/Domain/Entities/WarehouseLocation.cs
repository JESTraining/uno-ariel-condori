namespace WarehouseInventory.Api.Domain.Entities;

public class WarehouseLocation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Code { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

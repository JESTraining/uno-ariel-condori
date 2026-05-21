namespace WarehouseInventory.Api.Domain.Entities;

public class Category
{
    public required string Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

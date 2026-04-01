using ERPInventoryApi.Domain.Common;

namespace ERPInventoryApi.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    //Stock Keeping Unit - unique identifier for the product
    public string SKU { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int ReorderPoint { get; set; }

    public Guid CategoryId { get; set; }

    public Guid WarehouseId { get; set; }

    //Navigation properties
    public Category Category { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}

using ERPInventoryApi.Domain.Common;

namespace ERPInventoryApi.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Product> Products { get; set; } = [];
}

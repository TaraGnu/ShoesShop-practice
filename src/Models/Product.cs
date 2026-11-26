namespace ShoesShop.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ProductionId { get; set; }

    public string? Image { get; set; }

    public string? Descriptions { get; set; }

    public string? Material { get; set; }

    public float Price { get; set; }

    public int CategoryId { get; set; }

    public int SubcategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Production Production { get; set; } = null!;

    public virtual Subcategory Subcategory { get; set; } = null!;

    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}

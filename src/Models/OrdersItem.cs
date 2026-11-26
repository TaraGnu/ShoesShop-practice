namespace ShoesShop.Models;

public partial class OrdersItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ItemId { get; set; }

    public int Slotquantity { get; set; }

    public float Slotprice { get; set; }

    public virtual Warehouse Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}

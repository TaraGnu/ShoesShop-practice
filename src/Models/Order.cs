using System;
using System.Collections.Generic;

namespace ShoesShop.Models;

public partial class Order
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int UserId { get; set; }

    public float? TotalPrice { get; set; }

    public virtual ICollection<OrdersItem> OrdersItems { get; set; } = new List<OrdersItem>();

    public virtual User User { get; set; } = null!;
}

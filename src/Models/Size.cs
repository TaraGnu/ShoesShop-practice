using System;
using System.Collections.Generic;

namespace ShoesShop.Models;

public partial class Size
{
    public int Id { get; set; }

    public double Value { get; set; }

    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}

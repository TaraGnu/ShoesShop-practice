using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesShop.ViewModels
{
    public class OrderItemViewModel
    {
        public int Id { get; init; }
        public string ProductName { get; init; } = "";
        public string ProductionName { get; init; } = "";
        public double SizeValue { get; init; }
        public int Quantity { get; init; }
        public float UnitPrice { get; init; }
        public float TotalPrice { get; init; }

        public string Display => $"{ProductionName} | {ProductName} | размер {SizeValue:0.#}";
    }
}

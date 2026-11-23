using ShoesShop.Models;
using System.IO;

namespace ShoesShop.ViewModels
{
    public class ShoeCardViewModel
    {
        private readonly Product _product;

        public ShoeCardViewModel(Product product, bool hasOrdersInPreviousMonth = true)
        {
            _product = product;

            PriceWithDiscount = hasOrdersInPreviousMonth
                ? _product.Price
                : _product.Price * 0.75f;

            TotalQuantity = _product.Warehouses?.Sum(w => w.Quantity) ?? 0;
        }

        public int Id => _product.Id;
        public string Name => _product.Name;
        public string CategoryName => _product.Category.Name;
        public string ProductionName => _product.Production.Name ;
        public string? Material => _product.Material;

        public string ImagePath
        {
            get
            {
                var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/");
                if (!string.IsNullOrWhiteSpace(_product.Image))
                    if (File.Exists($"{baseDirectory}{_product.Image}"))
                        return Path.Combine(baseDirectory, _product.Image);
                return Path.Combine(baseDirectory, "picture.png");
            }
        }

        public int TotalQuantity { get; }
        public string QuantityText => TotalQuantity > 5 ? "много" : "мало";
        public float PriceWithDiscount { get; }
        public bool IsLowStock => TotalQuantity <= 3;

        public string ProductionAndName => $"{ProductionName} | {Name}";
    }
}

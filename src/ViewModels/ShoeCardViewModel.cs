using ShoesShop.Commands;
using ShoesShop.Models;
using System.IO;

namespace ShoesShop.ViewModels
{
    public class ShoeCardViewModel : ObservableObject
    {
        private readonly Product _product;

        public ShoeCardViewModel(Product product, bool hasOrdersInPreviousMonth)
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
        public string SubcategoryName => _product.Subcategory.Name;
        public string ProductionName => _product.Production.Name;
        public string? Material => _product.Material;
        public string? Description => _product.Descriptions;

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
        public float BasePrice
        {
            get
            {
                return _product.Price;
            }
        }
        public float PriceWithDiscount { get; }
        public bool HasDiscount => BasePrice != PriceWithDiscount;
        public bool IsLowStock => TotalQuantity <= 3;

        public string ProductionAndName => $"{ProductionName} | {Name}";

        public IReadOnlyList<SizeOption> AvailableSizes => _product.Warehouses
            .Where(w => w.Quantity > 0)
            .OrderBy(w => w.Size.Value)
            .Select(w => new SizeOption
            {
                WarehouseId = w.Id,
                SizeValue = w.Size.Value,
                Quantity = w.Quantity
            }).ToList();
        public string SizeRange
        {
            get
            {
                var sizes = _product.Warehouses
                    .Select(w => w.Size.Value)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToList();

                if (sizes.Count == 0) return "—";
                if (sizes.Count == 1) return sizes[0].ToString("0.#");
                return $"{sizes[0]:0.#} — {sizes[^1]:0.#}";
            }
        }
    }
}

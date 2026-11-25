using ShoesShop.Commands;

namespace ShoesShop.ViewModels
{
    public class CartItem : ObservableObject
    {
        public int WarehouseId { get; init; }
        public int ProductId { get; init; }
        public string ProductName { get; init; } = "";
        public string ProductionName { get; init; } = "";
        public double SizeValue { get; init; }
        public double UnitPrice { get; init; }
        public int AvailableStock { get; init; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, value))
                    OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public double TotalPrice => Quantity * UnitPrice;

        public string Display => $"{ProductionName} | {ProductName} | размер {SizeValue}";
    }
}

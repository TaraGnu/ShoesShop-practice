using ShoesShop.ViewModels;
using System.Collections.ObjectModel;

namespace ShoesShop.Service
{
    public static class CartService
    {
        public static ObservableCollection<CartItem> Items { get; } = new();

        public static event Action? Changed;

        public static double Total => Items.Sum(i => i.TotalPrice);

        public static void Add(CartItem item)
        {
            var existing = Items.FirstOrDefault(i => i.WarehouseId == item.WarehouseId);
            if (existing is not null)
                existing.Quantity = Math.Min(existing.AvailableStock, existing.Quantity + item.Quantity);
            else
                Items.Add(item);

            Changed?.Invoke();
        }

        public static void Remove(CartItem item)
        {
            if (Items.Remove(item))
                Changed?.Invoke();
        }

        public static void Clear()
        {
            Items.Clear();
            Changed?.Invoke();
        }
    }
}

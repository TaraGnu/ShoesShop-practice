using ShoesShop.Models;
using ShoesShop.ViewModels;

namespace ShoesShop.Service
{
    public static class OrderService
    {
        public static int CreateOrder(int userId, IEnumerable<CartItem> cartItems)
        {
            var items = cartItems.ToList();
            if (items.Count == 0)
                throw new InvalidOperationException("Заказ пуст.");

            using var context = new ShoesContext();
            using var tx = context.Database.BeginTransaction();

            var order = new Order { Date = DateTime.Now, UserId = userId };
            context.Orders.Add(order);
            context.SaveChanges();

            double total = 0;

            foreach (var cart in items)
            {
                var wh = context.Warehouses.FirstOrDefault(w => w.Id == cart.WarehouseId)
                    ?? throw new InvalidOperationException($"Позиция «{cart.Display}» больше не доступна.");

                if (wh.Quantity < cart.Quantity)
                    throw new InvalidOperationException(
                        $"Недостаточно товара «{cart.Display}». Доступно: {wh.Quantity}.");

                wh.Quantity -= cart.Quantity;

                context.OrdersItems.Add(new OrdersItem
                {
                    OrderId = order.Id,
                    ItemId = cart.WarehouseId,
                    Slotquantity = cart.Quantity,
                    Slotprice = (float)cart.UnitPrice
                });

                total += cart.TotalPrice;
            }

            order.TotalPrice = (float)total;
            context.SaveChanges();
            tx.Commit();

            return order.Id;
        }
    }
}
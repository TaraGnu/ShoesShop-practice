using Microsoft.EntityFrameworkCore;
using ShoesShop.Models;
using ShoesShop.ViewModels;

namespace ShoesShop.Service
{
    public static class OrderManagementService
    {
        public static List<OrderRowViewModel> GetOrders()
        {
            using var context = new ShoesContext();
            var orders = context.Orders.Include(o => o.User)
                .OrderByDescending(o => o.Date).ToList();

            return orders.Select(o => new OrderRowViewModel
            {
                Id = o.Id,
                Date = o.Date,
                UserFullName = o.User.ToString(),
                TotalPrice = o.TotalPrice ?? 0
            }).ToList();
        }

        public static List<User> GetClients()
        {
            using var context = new ShoesContext();
            return context.Users
                .Include(u => u.Role)
                .Where(u => u.Role.Name != "Manager"
                         && u.Role.Name != "Менеджер"
                         && u.Role.Name != "Admin"
                         && u.Role.Name != "Administrator"
                         && u.Role.Name != "Администратор")
                .OrderBy(u => u.Surname)
                .ToList();
        }

        public static Order? GetOrder(int orderId)
        {
            using var context = new ShoesContext();
            return context.Orders
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == orderId);
        }

        public static int CreateEmptyOrder(int userId, DateTime date)
        {
            using var context = new ShoesContext();
            var order = new Order
            {
                UserId = userId,
                Date = date,
                TotalPrice = 0
            };

            context.Orders.Add(order);
            context.SaveChanges();
            return order.Id;
        }

        public static void UpdateOrderDate(int orderId, DateTime date)
        {
            using var context = new ShoesContext();
            var order = context.Orders.FirstOrDefault(o => o.Id == orderId)
                ?? throw new InvalidOperationException("Заказ не найден.");

            order.Date = date;
            context.SaveChanges();
        }

        public static List<OrderItemViewModel> GetOrderItems(int orderId)
        {
            using var context = new ShoesContext();
            return context.OrdersItems
                .Include(i => i.Item).ThenInclude(w => w.Product).ThenInclude(p => p.Production)
                .Include(i => i.Item).ThenInclude(w => w.Size)
                .Where(i => i.OrderId == orderId)
                .Select(i => new OrderItemViewModel
                {
                    Id = i.Id,
                    ProductName = i.Item.Product.Name,
                    ProductionName = i.Item.Product.Production.Name,
                    SizeValue = i.Item.Size.Value,
                    Quantity = i.Slotquantity,
                    UnitPrice = i.Slotprice,
                    TotalPrice = i.Slotprice * i.Slotquantity
                })
                .ToList();
        }

        public static void AddOrderItem(int orderId, int warehouseId, int quantity, float unitPrice)
        {
            using var context = new ShoesContext();
            using var tx = context.Database.BeginTransaction();

            var order = context.Orders.FirstOrDefault(o => o.Id == orderId)
                ?? throw new InvalidOperationException("Заказ не найден.");

            var wh = context.Warehouses.FirstOrDefault(w => w.Id == warehouseId)
                ?? throw new InvalidOperationException("Товарная позиция не найдена.");

            if (wh.Quantity < quantity)
                throw new InvalidOperationException($"Недостаточно товара. Доступно: {wh.Quantity}.");

            wh.Quantity -= quantity;

            context.OrdersItems.Add(new OrdersItem
            {
                OrderId = orderId,
                ItemId = warehouseId,
                Slotquantity = quantity,
                Slotprice = unitPrice
            });

            context.SaveChanges();
            RecalculateTotal(context, orderId);
            tx.Commit();
        }

        public static void RemoveOrderItem(int orderItemId)
        {
            using var context = new ShoesContext();
            using var tx = context.Database.BeginTransaction();

            var item = context.OrdersItems.FirstOrDefault(i => i.Id == orderItemId)
                ?? throw new InvalidOperationException("Позиция заказа не найдена.");

            var wh = context.Warehouses.FirstOrDefault(w => w.Id == item.ItemId);
            if (wh is not null)
                wh.Quantity += item.Slotquantity;

            context.OrdersItems.Remove(item);
            context.SaveChanges();

            RecalculateTotal(context, item.OrderId);
            tx.Commit();
        }

        public static void DeleteOrder(int orderId)
        {
            using var context = new ShoesContext();
            using var tx = context.Database.BeginTransaction();

            var order = context.Orders
                .Include(o => o.OrdersItems)
                .FirstOrDefault(o => o.Id == orderId)
                ?? throw new InvalidOperationException("Заказ не найден.");

            foreach (var item in order.OrdersItems)
            {
                var wh = context.Warehouses.FirstOrDefault(w => w.Id == item.ItemId);
                if (wh is not null)
                    wh.Quantity += item.Slotquantity;

                context.OrdersItems.Remove(item);
            }

            context.Orders.Remove(order);
            context.SaveChanges();
            tx.Commit();
        }

        private static void RecalculateTotal(ShoesContext context, int orderId)
        {
            var order = context.Orders.First(o => o.Id == orderId);
            order.TotalPrice = context.OrdersItems
                .Where(i => i.OrderId == orderId)
                .Sum(i => i.Slotprice * i.Slotquantity);

            context.SaveChanges();
        }
    }
}

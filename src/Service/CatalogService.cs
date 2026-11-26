using Microsoft.EntityFrameworkCore;
using ShoesShop.Models;
using ShoesShop.ViewModels;

namespace ShoesShop.Service
{
    public static class CatalogService
    {
        public static List<ShoeCardViewModel> GetCatalog(DateTime calculationDate)
        {
            using var context = new ShoesContext();
            var productIdsWithOrders = GetProductIdsWithOrdersInPreviousMonth(context, calculationDate);
            var products = LoadProducts(context);
            return products.Select(p => new ShoeCardViewModel(p, productIdsWithOrders.Contains(p.Id))).ToList();
        }

        private static List<Product> LoadProducts(ShoesContext context)
        {
            return context.Products
                .Include(p => p.Category)
                .Include(p => p.Production)
                .Include(p => p.Subcategory)
                .Include(p => p.Warehouses).ThenInclude(w => w.Size)
                .ToList();
        }


        private static HashSet<int> GetProductIdsWithOrdersInPreviousMonth(ShoesContext context, DateTime calculationDate)
        {
            var currentMonthStart = new DateTime(calculationDate.Year, calculationDate.Month, 1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);

            return context.OrdersItems
                .Where(oi => oi.Order.Date >= previousMonthStart &&
                             oi.Order.Date < currentMonthStart)
                .Select(oi => oi.Item.ProductId)
                .Distinct()
                .ToHashSet();
        }
    }
}
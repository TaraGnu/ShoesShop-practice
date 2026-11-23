using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using ShoesShop.Models;
using ShoesShop.ViewModels;

namespace ShoesShop.Service
{
    public static class CatalogService
    {
        public static List<ShoeCardViewModel> GetCatalog()
        {
            using var dbcontext = new ShoesContext();
            var productIdsWithOrders = GetProductIdsWithOrdersInPreviousMonth(dbcontext);
            var products = LoadProducts(dbcontext);

            return BuildCards(products, productIdsWithOrders);
        }

        private static List<Product> LoadProducts(ShoesContext context)
        {
            return context.Products
                .Include(p => p.Category)
                .Include(p => p.Production)
                .Include(p => p.Warehouses)
                .ToList();
        }

        private static List<ShoeCardViewModel> BuildCards(
            IEnumerable<Product> products,
            HashSet<int> productIdsWithOrders)
        {
            return products
                .Select(p => new ShoeCardViewModel(p, productIdsWithOrders.Contains(p.Id)))
                .ToList();
        }
        private static HashSet<int> GetProductIdsWithOrdersInPreviousMonth(ShoesContext context)
        {
            var currentMonthStart = DateTime.Now;
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

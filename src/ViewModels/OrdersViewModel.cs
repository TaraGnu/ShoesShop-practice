using ShoesShop.Commands;
using ShoesShop.Service;
using System.Collections.ObjectModel;

namespace ShoesShop.ViewModels
{
    public class OrdersViewModel : ObservableObject
    {
        private readonly UserSession _session;

        public OrdersViewModel(UserSession session)
        {
            _session = session;
            Reload();
        }

        public ObservableCollection<OrderRowViewModel> Orders { get; } = new();
        public bool IsStaff => _session.IsStaff;

        public void Reload()
        {
            Orders.Clear();
            foreach (var order in OrderManagementService.GetOrders())
                Orders.Add(order);
        }
    }
}

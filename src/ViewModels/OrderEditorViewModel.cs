using ShoesShop.Commands;
using ShoesShop.Models;
using ShoesShop.Service;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ShoesShop.ViewModels
{
    public class OrderEditorViewModel : ObservableObject
    {
        private readonly UserSession _session;
        private int _orderId;
        private List<ShoeCardViewModel> _allProducts = new();
        public OrderEditorViewModel(int orderId, UserSession session)
        {
            _orderId = orderId;
            _session = session;

            IsAdmin = session.IsAdmin;
            IsStaff = session.IsStaff;

            CreateOrderCommand = new RelayCommand(
                _ => CreateEmptyOrder(),
                _ => IsNewOrder && SelectedClient is not null);

            SaveDateCommand = new RelayCommand(
                _ => SaveDate(),
                _ => IsExistingOrder && IsAdmin && OrderDate is not null);

            AddItemCommand = new RelayCommand(
                _ => AddItem(),
                _ => IsExistingOrder && IsStaff &&
                     SelectedProduct is not null &&
                     SelectedSize is not null &&
                     Quantity >= 1);

            RemoveItemCommand = new RelayCommand(
                p => RemoveItem(p as OrderItemViewModel),
                p => IsExistingOrder && IsAdmin && p is OrderItemViewModel);

            LoadClients();
            LoadProducts();

            if (_orderId > 0)
                LoadOrder();
            else
                OrderDate = DateTime.Today;
        }

        public bool IsAdmin { get; }
        public bool IsStaff { get; }

        public bool IsNewOrder => _orderId == 0;
        public bool IsExistingOrder => _orderId > 0;

        public string HeaderText => IsNewOrder ? "Новый заказ" : $"Заказ №{_orderId}";

        public ObservableCollection<User> Clients { get; } = new();
        public ObservableCollection<OrderItemViewModel> Items { get; } = new();
        public ObservableCollection<ShoeCardViewModel> Products { get; } = new();

        public ObservableCollection<SizeOption> AvailableSizesForProduct { get; } = new();

        private User? _selectedClient;
        public User? SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (SetProperty(ref _selectedClient, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        private DateTime? _orderDate;
        public DateTime? OrderDate
        {
            get => _orderDate;
            set
            {
                if (SetProperty(ref _orderDate, value))
                    CommandManager.InvalidateRequerySuggested();
            }
        }

        public DateTime MinOrderDate => new(2000, 1, 1);
        public DateTime MaxOrderDate => DateTime.Today.AddYears(1);

        private bool ValidateOrderDate(DateTime? date)
        {
            if (date is null)
            {
                MessageBox.Show(
                    "Укажите дату заказа.",
                    "Дата не задана",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (date.Value < MinOrderDate)
            {
                MessageBox.Show(
                    $"Дата заказа не может быть раньше {MinOrderDate:dd.MM.yyyy}.\n" +
                    "Проверьте правильность ввода даты.",
                    "Некорректная дата",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (date.Value > MaxOrderDate)
            {
                MessageBox.Show(
                    $"Дата заказа не может быть позже {MaxOrderDate:dd.MM.yyyy}.\n" +
                    "Проверьте правильность ввода даты.",
                    "Некорректная дата",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private string _searchText = "";
        public string SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) ApplyProductFilter(); }
        }

        private ShoeCardViewModel? _selectedProduct;
        public ShoeCardViewModel? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (!SetProperty(ref _selectedProduct, value))
                    return;

                ReloadAvailableSizes();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private SizeOption? _selectedSize;
        public SizeOption? SelectedSize
        {
            get => _selectedSize;
            set
            {
                if (!SetProperty(ref _selectedSize, value))
                    return;

                OnPropertyChanged(nameof(MaxQuantity));

                Quantity = Math.Min(Quantity, MaxQuantity);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public int MaxQuantity => SelectedSize?.Quantity ?? 1;

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                var clamped = Math.Max(1, Math.Min(value, MaxQuantity));
                SetProperty(ref _quantity, clamped);
            }
        }

        public double TotalPrice => Items.Sum(i => (double)i.TotalPrice);

        public bool ShowDateEditor => IsExistingOrder && IsAdmin;
        public bool ShowAddPanel => IsExistingOrder && IsStaff;
        public bool ShowDeleteButton => IsExistingOrder && IsAdmin;

        public ICommand CreateOrderCommand { get; }
        public ICommand SaveDateCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }

        private void LoadClients()
        {
            Clients.Clear();
            foreach (var client in OrderManagementService.GetClients())
                Clients.Add(client);
        }

        private void LoadProducts()
        {
            _allProducts = CatalogService.GetCatalog(DateTime.Now);
            ApplyProductFilter();
        }

        private void ApplyProductFilter()
        {
            Products.Clear();

            IEnumerable<ShoeCardViewModel> query = _allProducts;
            if (!string.IsNullOrWhiteSpace(SearchText))
                query = query.Where(p =>
                    p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var product in query.OrderBy(p => p.ProductionAndName))
                Products.Add(product);
        }

        private void LoadOrder()
        {
            var order = OrderManagementService.GetOrder(_orderId);
            if (order is null)
                return;

            SelectedClient = Clients.FirstOrDefault(c => c.Id == order.UserId);
            OrderDate = order.Date;
            ReloadOrder();
        }

        private void ReloadOrder()
        {
            Items.Clear();
            foreach (var item in OrderManagementService.GetOrderItems(_orderId))
                Items.Add(item);

            OnPropertyChanged(nameof(TotalPrice));
            CommandManager.InvalidateRequerySuggested();
        }


        private void ReloadAvailableSizes()
        {
            AvailableSizesForProduct.Clear();

            if (SelectedProduct is not null)
            {
                foreach (var size in SelectedProduct.AvailableSizes)
                    AvailableSizesForProduct.Add(size);
            }

            SelectedSize = AvailableSizesForProduct.FirstOrDefault();
        }

        private void CreateEmptyOrder()
        {
            if (SelectedClient is null)
            {
                MessageBox.Show("Выберите клиента.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateOrderDate(OrderDate))
                return;

            _orderId = OrderManagementService.CreateEmptyOrder(
                SelectedClient.Id,
                OrderDate!.Value);

            OnPropertyChanged(nameof(IsNewOrder));
            OnPropertyChanged(nameof(IsExistingOrder));
            OnPropertyChanged(nameof(HeaderText));
            OnPropertyChanged(nameof(ShowDateEditor));
            OnPropertyChanged(nameof(ShowAddPanel));
            OnPropertyChanged(nameof(ShowDeleteButton));

            ReloadOrder();
        }

        private void SaveDate()
        {
            if (!IsAdmin || OrderDate is null)
                return;

            OrderManagementService.UpdateOrderDate(_orderId, OrderDate.Value);
            MessageBox.Show("Дата заказа обновлена.", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddItem()
        {
            if (SelectedProduct is null || SelectedSize is null)
                return;

            var selectedProductId = SelectedProduct.Id;
            var selectedSizeId = SelectedSize.WarehouseId;

            try
            {
                OrderManagementService.AddOrderItem(
                    _orderId,
                    SelectedSize.WarehouseId,
                    Quantity,
                    SelectedProduct.PriceWithDiscount);

                ReloadOrder();
                LoadProducts();

                SelectedProduct = Products.FirstOrDefault(p => p.Id == selectedProductId);

                if (SelectedProduct is not null)
                {
                    var size = AvailableSizesForProduct
                        .FirstOrDefault(s => s.WarehouseId == selectedSizeId);
                    if (size is not null)
                        SelectedSize = size;
                }

                Quantity = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось добавить позицию: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RemoveItem(OrderItemViewModel? item)
        {
            if (item is null || !IsAdmin)
                return;

            var result = MessageBox.Show(
                $"Удалить позицию «{item.Display}» из заказа?\n" +
                "Количество будет возвращено на склад.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                OrderManagementService.RemoveOrderItem(item.Id);
                ReloadOrder();
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось удалить позицию: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}


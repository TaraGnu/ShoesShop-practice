using ShoesShop.Commands;
using ShoesShop.Service;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ShoesShop.ViewModels
{
    public class ProductViewModel : ObservableObject
    {
        private readonly ShoeCardViewModel _card;
        private readonly UserSession _session;

        public ProductViewModel(ShoeCardViewModel card, UserSession session)
        {
            _card = card ?? throw new ArgumentNullException(nameof(card));
            _session = session;

            AvailableSizes = new ObservableCollection<SizeOption>(_card.AvailableSizes);
            SelectedSize = AvailableSizes.FirstOrDefault();

            Cart = CartService.Items;
            CartService.Changed += OnCartChanged;

            AddToCartCommand = new RelayCommand(_ => AddToCart(), _ => CanAddToCart());
            RemoveFromCartCommand = new RelayCommand(p => RemoveFromCart(p as CartItem));
            ConfirmOrderCommand = new RelayCommand(_ => ConfirmOrder(), _ => CanConfirmOrder());
            ClearCartCommand = new RelayCommand(_ => CartService.Clear());
        }


        public string Name => _card.Name;
        public string ProductionName => _card.ProductionName;
        public string CategoryName => _card.CategoryName;
        public string SubcategoryName => _card.SubcategoryName;
        public string Material => string.IsNullOrWhiteSpace(_card.Material) ? "—" : _card.Material!;
        public string Description => string.IsNullOrWhiteSpace(_card.Description) ? "—" : _card.Description!;
        public string SizeRange => _card.SizeRange;
        public string ImagePath => _card.ImagePath;
        public float PriceWithDiscount => _card.PriceWithDiscount;
        public float BasePrice => _card.BasePrice;
        public bool HasDiscount => _card.HasDiscount;
        public bool HasAvailableSizes => AvailableSizes.Count > 0;
        public bool IsEmptySizes => !HasAvailableSizes;


        public ObservableCollection<SizeOption> AvailableSizes { get; }
        public ObservableCollection<CartItem> Cart { get; }

        private SizeOption? _selectedSize;
        public SizeOption? SelectedSize
        {
            get => _selectedSize;
            set
            {
                if (SetProperty(ref _selectedSize, value))
                    OnPropertyChanged(nameof(MaxQuantity));
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

        public double CartTotal => CartService.Total;
        public int CartCount => CartService.Items.Count;

        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand ConfirmOrderCommand { get; }
        public ICommand ClearCartCommand { get; }

        public event EventHandler? OrderCompleted;

        private bool CanAddToCart() => SelectedSize is not null && Quantity >= 1;

        private void AddToCart()
        {
            if (!_session.IsAuthenticated)
            {
                MessageBox.Show(
                    "Чтобы оформить заказ, войдите в систему под своей учётной записью.",
                    "Требуется авторизация",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedSize is null)
            {
                MessageBox.Show("Выберите размер перед добавлением в заказ.",
                    "Размер не выбран", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Quantity < 1 || Quantity > SelectedSize.Quantity)
            {
                MessageBox.Show(
                    $"Количество должно быть от 1 до {SelectedSize.Quantity}.",
                    "Некорректное количество", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CartService.Add(new CartItem
            {
                WarehouseId = SelectedSize.WarehouseId,
                ProductId = _card.Id,
                ProductName = Name,
                ProductionName = ProductionName,
                SizeValue = SelectedSize.SizeValue,
                UnitPrice = PriceWithDiscount,
                AvailableStock = SelectedSize.Quantity,
                Quantity = Quantity
            });

            Quantity = 1;
        }

        private void RemoveFromCart(CartItem? item)
        {
            if (item is null) return;
            CartService.Remove(item);
        }

        private bool CanConfirmOrder() =>
            _session.IsAuthenticated && CartService.Items.Count > 0;

        private void ConfirmOrder()
        {
            if (_session.CurrentUser is null)
                return;

            var result = MessageBox.Show(
                $"Оформить заказ на сумму {CartService.Total:N0} ₽?\n" +
                $"Позиций в заказе: {CartService.Items.Count}.",
                "Подтверждение заказа",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                OrderService.CreateOrder(_session.CurrentUser.Id, CartService.Items);
                CartService.Clear();

                MessageBox.Show("Заказ успешно оформлен.",
                    "Готово", MessageBoxButton.OK, MessageBoxImage.Information);

                OrderCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось оформить заказ: {ex.Message}\n" +
                    "Проверьте состав заказа и повторите попытку.",
                    "Ошибка оформления заказа",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCartChanged()
        {
            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(CartCount));
            CommandManager.InvalidateRequerySuggested();
        }
    }
}

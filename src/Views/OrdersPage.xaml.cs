using ShoesShop.Service;
using ShoesShop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ShoesShop.Views
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private readonly UserSession _session;
        private readonly OrdersViewModel _viewModel;

        public OrdersPage(UserSession session)
        {
            InitializeComponent();
            _session = session;
            _viewModel = new OrdersViewModel(session);
            DataContext = _viewModel;
            TopBar.Session = session;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Reload();
        }

        private void TopBar_LoginRequested(object sender, EventArgs e) =>
            TopBarNavigation.HandleLogin(this, _session);

        private void TopBar_LogoutRequested(object sender, EventArgs e) =>
            TopBarNavigation.HandleLogout(this, _session);

        private void TopBar_OrdersRequested(object sender, EventArgs e)
        {
            // Уже на странице заказов — просто обновляем.
            _viewModel.Reload();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new CatalogPage(_session));
        }

        private void Add_Click(object sender, RoutedEventArgs e) =>
            NavigationService.Navigate(new OrderEditorPage(0, _session));

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is OrderRowViewModel row)
                NavigationService.Navigate(new OrderEditorPage(row.Id, _session));
            else
                MessageBox.Show("Выберите заказ в списке.", "Заказ не выбран",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is not OrderRowViewModel row)
            {
                MessageBox.Show("Выберите заказ в списке.", "Заказ не выбран",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить заказ №{row.Id} от {row.Date:dd.MM.yyyy}?\n" +
                "Все товарные позиции будут возвращены на склад.",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                OrderManagementService.DeleteOrder(row.Id);
                _viewModel.Reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить заказ: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

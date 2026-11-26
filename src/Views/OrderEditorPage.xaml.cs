using ShoesShop.Service;
using ShoesShop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ShoesShop.Views
{
    public partial class OrderEditorPage : Page
    {
        private readonly UserSession _session;
        private readonly OrderEditorViewModel _viewModel;

        public OrderEditorPage(int orderId, UserSession session)
        {
            InitializeComponent();
            _session = session;
            _viewModel = new OrderEditorViewModel(orderId, session);
            DataContext = _viewModel;
            TopBar.Session = session;

            Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.IsAdmin && OrdersGrid.Columns.Count >= 5)
                OrdersGrid.Columns[4].Visibility = Visibility.Collapsed;
        }
        private void TopBar_LoginRequested(object sender, EventArgs e) =>
            TopBarNavigation.HandleLogin(this, _session);

        private void TopBar_LogoutRequested(object sender, EventArgs e) =>
            TopBarNavigation.HandleLogout(this, _session);

        private void TopBar_OrdersRequested(object sender, EventArgs e) =>
            NavigationService.Navigate(new OrdersPage(_session));
        private void TopBar_CatalogRequested(object sender, EventArgs e) =>
            NavigationService.Navigate(new CatalogPage(_session));

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new OrdersPage(_session));
        }

        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}
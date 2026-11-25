using ShoesShop.Service;
using ShoesShop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ShoesShop.Views
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private readonly UserSession _session;
        private readonly ProductViewModel _viewModel;

        public ProductPage(ShoeCardViewModel card, UserSession session)
        {
            InitializeComponent();
            _session = session;
            TopBar.Session = session;

            _viewModel = new ProductViewModel(card, session);
            _viewModel.OrderCompleted += (_, _) => NavigateBackToCatalog();
            DataContext = _viewModel;
        }

        private void TopBar_LoginRequested(object sender, EventArgs e) => TopBarNavigation.HandleLogin(this, _session);

        private void TopBar_LogoutRequested(object sender, EventArgs e) => TopBarNavigation.HandleLogout(this, _session);

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
                NavigationService.Navigate(new CatalogPage(_session));
        }

        private void NavigateBackToCatalog()
        {
            NavigationService.Navigate(new CatalogPage(_session));
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }

        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
    }
}

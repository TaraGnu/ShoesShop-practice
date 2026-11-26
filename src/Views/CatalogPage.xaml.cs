using ShoesShop.Service;
using ShoesShop.ViewModels;
using ShoesShop.Views.Controls;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShoesShop.Views
{
    /// <summary>
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        private readonly UserSession _session;

        public CatalogPage(UserSession session)
        {
            InitializeComponent();
            _session = session;
            DataContext = new CatalogViewModel(_session);
            TopBar.Session = session;
        }

        private void TopBar_LoginRequested(object sender, EventArgs e) => TopBarNavigation.HandleLogin(this, _session);
        private void TopBar_LogoutRequested(object sender, EventArgs e) => TopBarNavigation.HandleLogout(this, _session);
        private void TopBar_OrdersRequested(object sender, EventArgs e) => NavigationService.Navigate(new OrdersPage(_session));

        private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is ShoeCardControl { DataContext: ShoeCardViewModel card })
                NavigationService.Navigate(new ProductPage(card, _session));
        }
    }
}

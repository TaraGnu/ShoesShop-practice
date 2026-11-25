using ShoesShop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ShoesShop.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private readonly LoginViewModel _viewModel;
        private readonly UserSession _session;
        public LoginPage(UserSession session)
        {
            InitializeComponent();
            _viewModel = new LoginViewModel(session);
            _session = session;
            DataContext = _viewModel;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) => TryLogin();
        private void LoginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryLogin();
                e.Handled = true;
            }
        }
        private void TryLogin()
        {
            if (!_viewModel.Login())
                return;

            NavigationService.Navigate(new CatalogPage(_session));
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
        }
    }
}

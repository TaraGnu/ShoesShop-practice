using ShoesShop.Models;
using ShoesShop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShoesShop.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для TopBarControl.xaml
    /// </summary>
    public partial class TopBarControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(TopBarControl),
                new PropertyMetadata(string.Empty, OnTitleChanged));

        public static readonly DependencyProperty SessionProperty =
            DependencyProperty.Register(nameof(Session), typeof(UserSession), typeof(TopBarControl),
                new PropertyMetadata(null, OnSessionChanged));

        public TopBarControl()
        {
            InitializeComponent();
            Refresh();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public UserSession? Session
        {
            get => (UserSession?)GetValue(SessionProperty);
            set => SetValue(SessionProperty, value);
        }

        public event EventHandler? LoginRequested;
        public event EventHandler? LogoutRequested;
        public event EventHandler? OrdersRequested;

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TopBarControl control)
                control.TitleText.Text = (string)e.NewValue;
        }

        private static void OnSessionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TopBarControl control)
                control.Refresh();
        }

        public void Refresh()
        {
            var user = Session?.CurrentUser;

            OrdersButton.Visibility = Session?.IsStaff == true
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (user is null)
            {
                UserNameText.Text = "Гость";
                ActionButton.Content = "Войти";
            }
            else
            {
                UserNameText.Text = user.ToString();
                ActionButton.Content = "Выйти";
            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Session?.IsAuthenticated == true)
                LogoutRequested?.Invoke(this, EventArgs.Empty);
            else
                LoginRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            OrdersRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}

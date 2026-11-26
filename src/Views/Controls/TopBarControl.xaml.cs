using ShoesShop.ViewModels;
using System.Windows;
using System.Windows.Controls;

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
        public static readonly DependencyProperty ShowCatalogButtonProperty =
            DependencyProperty.Register(nameof(ShowCatalogButton), typeof(bool), typeof(TopBarControl),
                new PropertyMetadata(true, OnShowCatalogButtonChanged));
        public static readonly DependencyProperty ShowAuthButtonProperty =
            DependencyProperty.Register(nameof(ShowAuthButton), typeof(bool), typeof(TopBarControl),
                new PropertyMetadata(true, OnShowAuthButtonChanged));
        public static readonly DependencyProperty ShowOrdersButtonProperty =
            DependencyProperty.Register(nameof(ShowOrdersButton), typeof(bool), typeof(TopBarControl),
                new PropertyMetadata(true, OnShowOrdersButtonChanged));

        public bool ShowCatalogButton
        {
            get => (bool)GetValue(ShowCatalogButtonProperty);
            set => SetValue(ShowCatalogButtonProperty, value);
        }

        public bool ShowOrdersButton
        {
            get => (bool)GetValue(ShowOrdersButtonProperty);
            set => SetValue(ShowOrdersButtonProperty, value);
        }
        public bool ShowAuthButton
        {
            get => (bool)GetValue(ShowAuthButtonProperty);
            set => SetValue(ShowAuthButtonProperty, value);
        }

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
        public event EventHandler? CatalogRequested;

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TopBarControl control)
                control.TitleText.Text = (string)e.NewValue;
        }

        private static void OnShowOrdersButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TopBarControl c)
                c.UpdateOrdersButtonVisibility();
        }

        private static void OnSessionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TopBarControl control)
                control.Refresh();
        }
        private static void OnShowCatalogButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TopBarControl c)
                c.CatalogButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void OnShowAuthButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TopBarControl c)
                c.ActionButton.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }
        private void UpdateOrdersButtonVisibility()
        {
            OrdersButton.Visibility = (Session?.IsStaff == true && ShowOrdersButton)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public void Refresh()
        {
            var user = Session?.CurrentUser;

            UpdateOrdersButtonVisibility();

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

        private void OrdersButton_Click(object sender, RoutedEventArgs e) =>
            OrdersRequested?.Invoke(this, EventArgs.Empty);
        private void CatalogButton_Click(object sender, RoutedEventArgs e) =>
            CatalogRequested?.Invoke(this, EventArgs.Empty);
    }
}

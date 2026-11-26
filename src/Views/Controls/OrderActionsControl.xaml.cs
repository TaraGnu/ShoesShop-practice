using System.Windows;
using System.Windows.Controls;

namespace ShoesShop.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для OrderActionsControl.xaml
    /// </summary>
    public partial class OrderActionsControl : UserControl
    {
        public static readonly RoutedEvent EditRequestedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(EditRequested),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(OrderActionsControl));

        public static readonly RoutedEvent DeleteRequestedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DeleteRequested),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(OrderActionsControl));

        public OrderActionsControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler EditRequested
        {
            add => AddHandler(EditRequestedEvent, value);
            remove => RemoveHandler(EditRequestedEvent, value);
        }

        public event RoutedEventHandler DeleteRequested
        {
            add => AddHandler(DeleteRequestedEvent, value);
            remove => RemoveHandler(DeleteRequestedEvent, value);
        }

        private void Edit_Click(object sender, RoutedEventArgs e) =>
            RaiseEvent(new RoutedEventArgs(EditRequestedEvent, this));

        private void Delete_Click(object sender, RoutedEventArgs e) =>
            RaiseEvent(new RoutedEventArgs(DeleteRequestedEvent, this));
    }
}

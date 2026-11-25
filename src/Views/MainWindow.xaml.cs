using ShoesShop.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ShoesShop.Views;

public partial class MainWindow : Window
{
    private readonly UserSession _session  = new();

    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigated += (_, e) =>
        {
            if (e.Content is Page page)
                Title = $"{page.Title} — Магазин обуви";
        };
        this.Icon = new BitmapImage(new Uri("pack://application:,,,/ShoesIco.ico", UriKind.Absolute));
        MainFrame.Navigate(new CatalogPage(_session));
    }

}
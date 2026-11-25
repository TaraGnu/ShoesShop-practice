using ShoesShop.ViewModels;
using System.Windows;
using System.Windows.Controls;

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

        MainFrame.Navigate(new CatalogPage(_session));
    }

}